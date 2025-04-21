using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotCommon;
using BotCommon.Broadcast;
using BotCommon.Repository;
using BotCommon.Scenarios;
using DirectumCareerNightBot.Scenarios;
using Newtonsoft.Json;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot;

public class BotUpdateHandler : IUpdateHandler
{
    private static readonly ILogger log = LogManager.GetCurrentClassLogger();

    private static UserScenarioRepository? _userScenarioRepository;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        log.Trace(JsonConvert.SerializeObject(update));
        var userInfo = BotHelper.GetUserInfo(update);
        log.Info($"user: {BotHelper.GetUsername(userInfo)}, userMessage: {BotHelper.GetMessage(update)}");
        var userId = userInfo.Id;

        BotDbContext.Instance.Add(new BotUser(
            userId, 
            userInfo.Username,
            userInfo.FirstName,
            userInfo.LastName,
            userInfo.LanguageCode));

        UserCommandScenario? userScenario = null;
        switch (BotHelper.GetMessage(update))
        {
            case BotChatCommands.Start:
            {
                var replyMarkup = MainMenuCommand.GetMainMenuInlineMarkup();
                await botClient.SendTextMessageAsync(userId, 
                    BotMessages.BotStartMessage, 
                    replyMarkup: replyMarkup, 
                    cancellationToken: cancellationToken,
                    parseMode: ParseMode.MarkdownV2);
                _userScenarioRepository?.Remove(userId);
                return;
            }
            case BotChatCommands.MainMenu:
            {
                var replyMarkup = MainMenuCommand.GetMainMenuInlineMarkup();
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await botClient.SendTextMessageAsync(userId,
                        BotMessages.MainMenu,
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken,
                        parseMode: ParseMode.MarkdownV2);
                    _userScenarioRepository?.Remove(userId);
                }
                else if (update.Type == UpdateType.Message)
                {
                    await botClient.SendTextMessageAsync(userId,
                        BotMessages.MainMenu,
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken,
                        parseMode: ParseMode.MarkdownV2);
                    _userScenarioRepository?.Remove(userId);
                }
                return;
            }
            case BotChatCommands.Student:
            {
                var buttons = new List<InlineKeyboardButton[]>
                {
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.WantToPractice, BotChatCommands.WantToPractice), },
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.StudentWithExperience, BotChatCommands.StudentWithExperience), },
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.Directum15Questions, BotChatCommands.Directum15Questions), },
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu), },
                };

                var replyMarkup = new InlineKeyboardMarkup(buttons);
                await botClient.EditMessageTextAsync(userId,
                    update.CallbackQuery.Message.MessageId,
                    BotMessages.StudentMessage,
                    replyMarkup: replyMarkup,
                    cancellationToken: cancellationToken,
                    parseMode: ParseMode.MarkdownV2);
                _userScenarioRepository?.Remove(userId);
                break;
            }
            case BotChatCommands.NotStudent:
            {
                var buttons = new List<InlineKeyboardButton[]>
                {
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.WantToIT, BotChatCommands.WantToIT), },
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.WantInterview, BotChatCommands.WantInterview), },
                    new [] { InlineKeyboardButton.WithUrl(BotMessages.Vacancies, "https://career.directum.ru/vacancy"), },
                    new [] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu), },
                };

                var replyMarkup = new InlineKeyboardMarkup(buttons);
                await botClient.EditMessageTextAsync(userId,
                    update.CallbackQuery.Message.MessageId,
                    BotMessages.NotStudentMessage,
                    replyMarkup: replyMarkup,
                    cancellationToken: cancellationToken,
                    parseMode: ParseMode.MarkdownV2);
                _userScenarioRepository?.Remove(userId);
                break;
            }
            case BotChatCommands.WantToPractice:
                userScenario = new UserCommandScenario(userId, new StudentPracticeScenario());
                break;
            case BotChatCommands.WantInterview:
                userScenario = new UserCommandScenario(userId, new InterviewScenario());
                break;
            case BotChatCommands.StudentWithExperience:
                userScenario = new UserCommandScenario(userId, new StudentWorkScenario());
                break;
            case BotChatCommands.WantToIT:
                userScenario = new UserCommandScenario(userId, new WantToITScenario());
                break;
            case BotChatCommands.Directum15Questions:
            {
                var buttons = new List<InlineKeyboardButton[]>
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu),
                    }
                };
                var markup = new InlineKeyboardMarkup(buttons);
                await botClient.SendPhotoAsync(userId,
                    caption: BotMessages.TraineeITMan,
                    photo: InputFile.FromStream(System.IO.File.OpenRead("Scenarios\\Images\\Есть контакт.jpg")),
                    replyMarkup: markup,
                    cancellationToken: cancellationToken,
                    parseMode: ParseMode.MarkdownV2);
                break;
            }
            case BotChatCommands.Quiz:
                userScenario = new UserCommandScenario(userId, new QuizScenario());
                break;
        }
        if (userScenario == null && _userScenarioRepository.TryGet(userId, out var _userScenario))
            userScenario = _userScenario;
        else
            _userScenarioRepository.AddOrReplace(userScenario);

        if (userScenario != null && !(await userScenario.Run(botClient, update, userId)))
            _userScenarioRepository.Remove(userScenario);
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        log.Error(exception);
        Environment.Exit(0);
    }

    public BotUpdateHandler()
    {
        _userScenarioRepository = new UserScenarioRepository();
    }
}