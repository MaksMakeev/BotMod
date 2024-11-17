using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotMod.Telegram
{
    
    public class ModelGenerator
    {
        public Dictionary<long, ModelState> ChatDict { get; set; } = new();

        public async Task Process(ITelegramBotClient client, Update update, ModelController modelController, CancellationToken ct)
        {
            if (!ChatDict.TryGetValue(update.Message!.Chat.Id, out var state))
            {
                ChatDict.Add(update.Message!.Chat.Id, new ModelState());
            }

            state = ChatDict[update.Message!.Chat.Id];

            switch (state.Mode)
            {
                case Mode.Initial:
                    await SendInitial(client, update, state, ct);
                    break;
                case Mode.SetName:
                    await SendSetModelName(client, update, state, ct);
                    break;
                case Mode.SetAge:
                    await SendSetModelAge(client, update, state, ct);
                    break;
                case Mode.SetIsPlusSize:
                    await SendSetIsPlusSize(client, update, state, ct);
                    break;
                case Mode.SetContact:
                    await SendSetModelContact(client, update, state, ct);
                    break;
                case Mode.SetSex:
                    await SendSetModelSex(client, update, state, ct);
                    break;
                case Mode.SetComment:
                    await SendSetComment(client, update, state, modelController, ct);
                    break;
                case Mode.SetRetry:
                    await SendSetRetry(client, update, state, ct);
                    break;
            }

        }

        private static async Task SendSetRetry(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var isRetry = update.Message.Text;
            if (isRetry == "Да")
            {
                await client.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "Введи имя модели",
                    cancellationToken: ct);
                state.Mode = Mode.SetName;
            }
            else
            {
                state.Mode = Mode.Initial;

                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: $"Модель {state.Name}\r\n{state.Age}\r\n{state.IsPlusSize}\r\n{state.Contact}\r\n{state.Sex}\r\n{state.Comment}\r\nуспешно сохранена!",
                   cancellationToken: ct);

                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: $"\r\n\r\n Жмакни:\r\n/exit - чтобы вернуться в главное меню\r\n/end - чтобы попращаться с ботом",
                   cancellationToken: ct);

                return;
            }
        }

        private static async Task SendSetComment(ITelegramBotClient client, Update update, ModelState state, ModelController modelController, CancellationToken ct)
        {
            state.Comment = update.Message.Text;

            modelController.CreateModel(state.Name, state.Age, state.IsPlusSize, state.Contact, state.Sex, state.Comment);

            var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Да"),
                                new KeyboardButton("Нет"),
                            }
                        })
            {
                ResizeKeyboard = true,
            };

            await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Создать еще контакт?",
                   replyMarkup: replyKeyboard,
                   cancellationToken: ct);
            state.Mode = Mode.SetRetry;
        }

        private static async Task SendSetModelSex(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            state.Sex = update.Message.Text;
            await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи комментарий",
                   cancellationToken: ct);
            state.Mode = Mode.SetComment;
        }

        private static async Task SendSetModelContact(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var modelContact = update.Message.Text;
            if (modelContact == null)
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи контакт повторно",
                   cancellationToken: ct);
            }
            else
            {
                state.Contact = modelContact;

                var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Женский"),
                                new KeyboardButton("Мужской"),
                            }
                        })
                {
                    ResizeKeyboard = true,
                };

                await client.SendTextMessageAsync(
                      chatId: update.Message!.Chat.Id,
                      text: "Выбери пол модели",
                      replyMarkup: replyKeyboard,
                      cancellationToken: ct);
                state.Mode = Mode.SetSex;
            }
        }

        private static async Task SendSetIsPlusSize(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var isPlusSize = update.Message.Text;
            if (isPlusSize == "Да")
            {
                state.IsPlusSize = true;
            }
            else
            {
                state.IsPlusSize = false;
            }
            await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи номер телефона, e-mail или любой другой контакт модели",
                   cancellationToken: ct);
            state.Mode = Mode.SetContact;
        }

        private static async Task SendSetModelAge(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var modelAge = update.Message!.Text;
            if (!int.TryParse(modelAge, out var age))
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи возраст повторно",
                   cancellationToken: ct);
            }
            else
            {
                state.Age = age;

                var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Да"),
                                new KeyboardButton("Нет"),
                            }
                        })
                {
                    ResizeKeyboard = true,
                };

                await client.SendTextMessageAsync(
                      chatId: update.Message!.Chat.Id,
                      text: "Модель +Size?",
                      replyMarkup: replyKeyboard,
                      cancellationToken: ct);
                state.Mode = Mode.SetIsPlusSize;
            }
        }

        private static async Task SendSetModelName(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var modelName = update.Message.Text;
            if (modelName == null)
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи имя повторно",
                   cancellationToken: ct);
            }
            else
            {
                state.Name = modelName;
                await client.SendTextMessageAsync(
                      chatId: update.Message!.Chat.Id,
                      text: "Введи возраст модели",
                      cancellationToken: ct);
                state.Mode = Mode.SetAge;
            }
        }

        private static async Task SendInitial(ITelegramBotClient client, Update update, ModelState? state, CancellationToken ct)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                text: "Введи имя модели",
                cancellationToken: ct);
            state.Mode = Mode.SetName;
        }

    }
}
