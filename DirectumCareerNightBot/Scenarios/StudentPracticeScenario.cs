using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using BotCommon.Scenarios;
using DirectumCareerNightBot.GoogleSheets;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace DirectumCareerNightBot.Scenarios;

public class StudentPracticeScenario : AutoStepBotCommandScenario
{
    public override Guid Id { get; } = new Guid("87036BA3-710B-4742-86E0-A1FD8F699741");
    public override string ScenarioCommand => string.Empty;

    private async Task StepAction1(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUserInfo = BotHelper.GetUserInfo(update);
        var userData = new UserData
        {
            TelegramName = string.IsNullOrEmpty(botUserInfo.Username)
                ? $"{botUserInfo.FirstName} {botUserInfo.LastName}"
                : botUserInfo.Username,
            UserId = botUserInfo.Id,
            Fullname = string.Empty,
            Contact = string.Empty,
            SomeField = string.Empty,
            Experience = string.Empty
        };
        BotDbContext.Instance.UserDatas.Add(userData);
        await BotDbContext.Instance.SaveChangesAsync();

        var replyMarkup = new ReplyKeyboardRemove();
        await bot.SendTextMessageAsync(
            chatId,
            BotMessages.IntroduceYourself,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: replyMarkup);
    }
    private async Task StepAction2(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Fullname = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();
        
        var markup = new ReplyKeyboardMarkup(Directions.GetDirectionsKeyBoard());
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.InterestingDirection, 
            replyMarkup: markup,
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction3(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.SomeField = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();
        
        var replyMarkup = new ReplyKeyboardRemove();
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.HowToContact,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: replyMarkup);
    }
    private async Task StepAction4(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Contact = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();

        await bot.SendTextMessageAsync(chatId, 
            BotMessages.TellAboutChosenDirection, 
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction5(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Experience = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();

        var sheetManager = new GoogleSheetsManager();
        sheetManager.AddUserToTraineeSheet(userData.Fullname, userData.Contact, userData.SomeField, userData.Experience, userData.TelegramName);
        
        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithUrl(BotMessages.DirectumStudentsVK, "https://vk.com/student_directum") },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu) }
        };
        var markup = new InlineKeyboardMarkup(buttons);
        await bot.SendPhotoAsync(chatId, 
            photo: InputFile.FromStream(File.OpenRead("Scenarios\\Images\\Практика.jpg")), 
            caption: BotMessages.ThankYouPractice, 
            replyMarkup: markup,
            parseMode: ParseMode.MarkdownV2);
    }

    public StudentPracticeScenario()
    {
        this.steps = new List<BotCommandScenarioStep>
        {
            new (StepAction1),
            new (StepAction2),
            new (StepAction3),
            new (StepAction4),
            new (StepAction5),

        }.GetEnumerator();
    }
}