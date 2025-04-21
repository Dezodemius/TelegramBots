using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using BotCommon.Scenarios;
using DirectumCareerNightBot.GoogleSheets;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot.Scenarios;

public class StudentWorkScenario : AutoStepBotCommandScenario
{
    public override Guid Id { get; } = new ("A00C9F14-6B9E-4421-9D14-E73A1E73EB2F");
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

        var buttons = new List<InlineKeyboardButton[]>
        {
            new []{ InlineKeyboardButton.WithCallbackData("Да", "Yes")},
            new []{ InlineKeyboardButton.WithCallbackData("Нет", "No")}
        };
        var markup = new InlineKeyboardMarkup(buttons);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId,
            BotMessages.NotStudentMessage,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: markup);
    }   
    private async Task StepAction25(ITelegramBotClient bot, Update update, long chatId)
    {
        var userChoice = BotHelper.GetMessage(update);
        if (userChoice == "Yes")
        {
            var replyMarkup = new ReplyKeyboardRemove();
            await bot.SendTextMessageAsync(chatId, 
                BotMessages.IntroduceYourself,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup);
        }
        else if (userChoice == "No")
        {
            this.steps = new List<BotCommandScenarioStep>().GetEnumerator();
            var buttons = new List<InlineKeyboardButton[]>
            {
                new []{ InlineKeyboardButton.WithUrl(BotMessages.DirectumStudentsVK, "https://vk.com/student_directum")},
                new []{ InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu)}
            };
            var markup = new InlineKeyboardMarkup(buttons);
            await bot.SendPhotoAsync(chatId, 
                caption: BotMessages.TraineeITMan,
                photo: InputFile.FromStream(System.IO.File.OpenRead("Scenarios\\Images\\Есть контакт.jpg")),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: markup);
        }
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
        await bot.SendTextMessageAsync(chatId, BotMessages.InterestingDirection,
            parseMode: ParseMode.MarkdownV2, replyMarkup: markup);
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
        
        var markup = new ReplyKeyboardMarkup(Directions.GetDirectionsKeyBoard());
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.TellAboutITExpirience,
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: markup);
    }

    private async Task StepAction4(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Experience = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();
        
        await bot.SendTextMessageAsync(chatId, 
            BotMessages.HowToContact,
            parseMode: ParseMode.MarkdownV2);
    }

    private async Task StepAction5(ITelegramBotClient bot, Update update, long chatId)
    {
        var user = BotHelper.GetUserInfo(update);
        var userData = BotDbContext.Instance.UserDatas
            .Where(u => u.UserId == user.Id)
            .OrderByDescending(d => d.Id)
            .First();
        userData.Contact = BotHelper.GetMessage(update);
        await BotDbContext.Instance.SaveChangesAsync();

        var sheetManager = new GoogleSheetsManager();
        sheetManager.AddUserToInterviewSheet(userData.Fullname, userData.Contact, userData.SomeField, userData.Experience, userData.TelegramName);

        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithUrl(BotMessages.DirectumCompanyVK, "https://vk.com/directum_people") },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu) }
        };
        var markup = new InlineKeyboardMarkup(buttons);
        if (Directions.AllDirections.Contains(userData.SomeField))
        {
            await bot.SendPhotoAsync(chatId, 
                caption: string.Format(BotMessages.WaitingForYou, Directions.GetDirectionAlias(userData.SomeField)),
                photo: InputFile.FromStream(System.IO.File.OpenRead($"Scenarios\\Images\\{Directions.GetDirectionAlias(userData.SomeField)}.jpg")),
                replyMarkup: markup,
                parseMode: ParseMode.MarkdownV2);
        }
        else
        {
            await bot.SendPhotoAsync(chatId, 
                caption: BotMessages.ThankYouInITAnother,
                photo: InputFile.FromStream(System.IO.File.OpenRead("Scenarios\\Images\\Есть контакт.jpg")),
                replyMarkup: markup,
                parseMode: ParseMode.MarkdownV2);
        }
    }
    public StudentWorkScenario()
    {
        this.steps = new List<BotCommandScenarioStep>
        {
            new (StepAction1),
            new (StepAction25),
            new (StepAction2),
            new (StepAction3),
            new (StepAction4),
            new (StepAction5),

        }.GetEnumerator();
    }
}