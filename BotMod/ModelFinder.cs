using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotMod.Telegram
{
    public class ModelFinder
    {
        public Dictionary<long, ModelState> ChatDict { get; set; } = new();

        public static Dictionary<string, string> Filters { get; set; } = new();

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
                case Mode.SetFilterChoise:
                    await SendSetFilterChoise(client, update, state, ct);
                    break;
                case Mode.SetFilterSex:
                    await SendSetFilterSex(client, update, state, ct);
                    break;
                case Mode.SetLowAgeBorder:
                    await SendSetLowAgeBorder(client, update, state, ct);
                    break;
                case Mode.SetUpperAgeBorder:
                    await SendSetUpperAgeBorder(client, update, state, ct);
                    break;
                case Mode.SetFilterAddition:
                    await SendSetFilterAddition(client, update, state, modelController, ct);
                    break;
            }

        }

        private static async Task SendSetFilterAddition(ITelegramBotClient client, Update update, ModelState state, ModelController modelController, CancellationToken ct)
        {
            var isRetry = update.Message.Text;
            if (isRetry == "Да")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(
                         new List<KeyboardButton[]>()
                         {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Пол"),
                                new KeyboardButton("Возраст"),
                            }
                         })
                {
                    ResizeKeyboard = true,
                };

                await client.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "По какому параметру хочешь искать модель?",
                    replyMarkup: replyKeyboard,
                    cancellationToken: ct);
                state.Mode = Mode.SetFilterChoise;
            }
            else
            {
                string? sex;
                int? ageFrom;
                int? ageTo;

                if (Filters.ContainsKey("sex"))
                {
                    sex = Filters["sex"];
                }
                else
                {
                    sex = null;
                }
                if (Filters.ContainsKey("lowAgeBorder"))
                {
                    int.TryParse(Filters["lowAgeBorder"], out var lowAge);
                    ageFrom = lowAge;
                }
                else
                {
                    ageFrom = null;
                }
                if (Filters.ContainsKey("upperAgeBorder"))
                {
                    int.TryParse(Filters["upperAgeBorder"], out var upperAge);
                    ageTo = upperAge;
                }
                else
                {
                    ageTo = null;
                }

                var models = modelController.GetModelsBySexOrAge(sex, ageFrom, ageTo);

                if (models.Count() > 0)
                {
                    foreach (var model in models)
                    {
                        await client.SendTextMessageAsync(
                           chatId: update.Message!.Chat.Id,
                           text: $"Вот, что мне удалось найти: {model.Name}\r\n{model.Age}\r\n{model.IsPlusSize}\r\n{model.Contact}\r\n{model.Sex}\r\n{model.Comment}",
                           cancellationToken: ct);
                    }
                }
                else
                {
                    await client.SendTextMessageAsync(
                           chatId: update.Message!.Chat.Id,
                           text: "Сорян, но мне не удалось ничего найти",
                           cancellationToken: ct);
                }

                client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                text: "Жмакни:\r\n/exit - чтобы вернуться в главное меню\r\n/end - чтобы попращаться с ботом"
                );
                Filters.Clear();
                state.Mode = Mode.Initial;
                return;
            }
        }
        private static async Task SendSetUpperAgeBorder(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var upperAgeBorder = update.Message!.Text;
            if (!int.TryParse(upperAgeBorder, out var upperAge))
            {
                await client.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                   text: "Введи возраст повторно",
                   cancellationToken: ct);
            }
            else
            {
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
                     text: "Хочешь добавить еще условие поиска?",
                     replyMarkup: replyKeyboard,
                     cancellationToken: ct);
                state.Mode = Mode.SetFilterAddition;

                if (Filters.ContainsKey("upperAgeBorder"))
                {
                    Filters.Add("upperAgeBorder", upperAge.ToString());
                }
                else
                {
                    Filters["upperAgeBorder"] = upperAge.ToString();
                }
            }
        }

        private static async Task SendSetLowAgeBorder(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var lowAgeBorder = update.Message!.Text;
            if (!int.TryParse(lowAgeBorder, out var lowAge))
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message!.Chat.Id,
                   text: "Введи возраст повторно",
                   cancellationToken: ct);
            }
            else
            {
                await client.SendTextMessageAsync(
                     chatId: update.Message!.Chat.Id,
                     text: "До какого возраста, ты ищешь модель?",
                     cancellationToken: ct);
                state.Mode = Mode.SetUpperAgeBorder;

                if (Filters.ContainsKey("lowAgeBorder"))
                {
                    Filters.Add("lowAgeBorder", lowAge.ToString());
                }
                else
                {
                    Filters["lowAgeBorder"] = lowAge.ToString();
                }
            }
        }

        private static async Task SendSetFilterSex(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
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
                      text: "Хочешь добавить еще условие поиска?",
                      replyMarkup: replyKeyboard,
                      cancellationToken: ct);
            state.Mode = Mode.SetFilterAddition;

            if (Filters.ContainsKey("sex"))
            {
                Filters.Add("sex", update.Message!.Text);
            }
            else
            {
                Filters["sex"] = update.Message!.Text;
            }
        }

        private static async Task SendSetFilterChoise(ITelegramBotClient client, Update update, ModelState state, CancellationToken ct)
        {
            var filter = update.Message.Text;
            if (filter == "Пол")
            {
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
                   text: "Модель какого пола ты ищешь?",
                   replyMarkup: replyKeyboard,
                   cancellationToken: ct);
                state.Mode = Mode.SetFilterSex;
            }
            else
            {
                await client.SendTextMessageAsync(
                      chatId: update.Message!.Chat.Id,
                      text: "Начиная от какого возраста, ты ищешь модель?",
                      cancellationToken: ct);
                state.Mode = Mode.SetLowAgeBorder;
            }
        }

        private static async Task SendInitial(ITelegramBotClient client, Update update, ModelState? state, CancellationToken ct)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                        new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                            {
                                new KeyboardButton("Пол"),
                                new KeyboardButton("Возраст"),
                            }
                        })
            {
                ResizeKeyboard = true,
            };

            await client.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                text: "По какому параметру хочешь искать модель?",
                replyMarkup: replyKeyboard,
                cancellationToken: ct);
            state.Mode = Mode.SetFilterChoise;
        }

    }
}
