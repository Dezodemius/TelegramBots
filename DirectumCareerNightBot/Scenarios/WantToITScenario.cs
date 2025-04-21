using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using BotCommon.Scenarios;
using DirectumCareerNightBot.GoogleSheets;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot.Scenarios;

internal class WantToITScenario : AutoStepBotCommandScenario
{
    public override Guid Id { get; } = new Guid("5C7B454C-0B27-48A1-9769-A1D93EB6450B");
    public override string ScenarioCommand { get; }
    
    public async Task StepAction1(ITelegramBotClient bot, Update update, long chatId)
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
    public async Task StepAction2(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Fullname = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();
        
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.HowToContact,
            parseMode: ParseMode.MarkdownV2);
    }
    public async Task StepAction3(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Contact = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();

        var replyMarkup = new ReplyKeyboardMarkup(Directions.GetDirectionsKeyBoard());
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.InterestingDirection,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: replyMarkup);
    }
    public async Task StepAction4(ITelegramBotClient bot, Update update, long chatId)
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
            BotMessages.TellAboutLastWork,
            parseMode: ParseMode.MarkdownV2, 
            replyMarkup: replyMarkup);
    }
    public async Task StepAction5(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Experience = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();
        
        var sheetManager = new GoogleSheetsManager();
        sheetManager.AddUserToMatureSheet(userData.Fullname, userData.Contact, userData.SomeField, userData.Experience, userData.TelegramName);

        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithUrl(BotMessages.DirectumStudentsVK, "https://vk.com/student_directum") },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu) },
        };
        var markup = new InlineKeyboardMarkup(buttons);
        await bot.SendPhotoAsync(chatId, 
            caption: BotMessages.GoUpInIT, 
            photo: InputFile.FromStream(System.IO.File.OpenRead("Scenarios\\Images\\Попасть в IT.jpg")),
            replyMarkup: markup,
            parseMode: ParseMode.MarkdownV2);
    }
    public WantToITScenario()
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