using BotMod;
using BotMod.Telegram;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


var modelGenerator = new ModelGenerator();
var modelFinder = new ModelFinder();
var modelController = new ModelController();
var stateOfChat = new Dictionary<long, ChatMode>();

var ro = new ReceiverOptions
{
    AllowedUpdates = [],

};


var botClient = new TelegramBotClient("7027849636:AAH05MgMgNHJeoIjIQiDebUiXen_W5Hf8Pg");

botClient.StartReceiving(
        Handler,
        ErrorHandler,
        ro
    );


var me = await botClient.GetMeAsync();
Console.WriteLine($"{me.FirstName} has been started!");

await Task.Delay(-1);



async Task Handler(ITelegramBotClient client, Update update, CancellationToken ct)
{
    var message = update.Message;
    var user = message.From;
    Console.WriteLine($"A message from {user.FirstName} ({user.Id}) has been received! Message: {message.Text}");

    if (update.Message == null)
    {
        return;
    }

    if (update.Message.Text == "/start")
    {
        stateOfChat[update.Message!.Chat.Id] = ChatMode.Initial;

        await SendMenu(client, update);
    }

    if (!stateOfChat.TryGetValue(update.Message!.Chat.Id, out var state))
    {
        stateOfChat.Add(update.Message!.Chat.Id, ChatMode.Initial);
    }

    state = stateOfChat[update.Message!.Chat.Id];

    if (update.Message.Text == "/end")
    {
        stateOfChat[update.Message!.Chat.Id] = ChatMode.Initial;

        await client.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "Пока!",
                    cancellationToken: ct);
    }

    if (update.Message.Text == "/exit")
    {
        stateOfChat[update.Message!.Chat.Id] = ChatMode.Initial;

        await SendMenu(client, update);
    }
    else
    {
        switch (state)
        {
            case ChatMode.Create:
                await modelGenerator.Process(client, update, modelController, ct);
                break;

            case ChatMode.Find:
                await modelFinder.Process(client, update, modelController, ct);
                break;

            default:
                switch (update.Message.Text)
                {
                    case "Создать контакт":
                        await modelGenerator.Process(client, update, modelController, ct);
                        stateOfChat[update.Message!.Chat.Id] = ChatMode.Create;
                        break;

                    case "Найти модель":
                        await modelFinder.Process(client, update, modelController, ct);
                        stateOfChat[update.Message!.Chat.Id] = ChatMode.Find;
                        break;

                    default:
                        //await SendMenu(client, update);
                        break;
                }
                break;
        }


    }


}

async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
{
    Console.WriteLine("");
}

static Task SendMenu(ITelegramBotClient client, Update update)
{
    var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Найти модель"),
                                new KeyboardButton("Создать контакт"),
                            }
                        })
    {
        ResizeKeyboard = true,
    };
    return client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
          text: "Привет!\r\n" +
          "Добро пожаловать в BotMod!\r\n\r\n" +
          "Здесь ты сможешь сохранить свою базу моделей для съемок, " +
          "просматривать их контактную информацию, чтобы быть с ними на связи, " +
          "оставлять заметки и добавлять в избранное тех, " +
          "с кем хотелось бы поработать еще.\r\n\r\n" +
          "С чего хочешь начать?",
          replyMarkup: replyKeyboard
          );
}

enum ChatMode
{
    Initial = 0,
    Create = 1,
    Update = 2,
    Find = 3,
}

