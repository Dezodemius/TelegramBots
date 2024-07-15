﻿using BotCommon;
using BotCommon.Scenarios;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCoffee;

public class ChangeNameScenario : AutoStepBotCommandScenario
{
    public override Guid Id { get; } = new Guid("EE3FCEBF-DFF8-458F-9029-D60466DA72D6");
    public override string ScenarioCommand { get; }

    private async Task StepAction1(ITelegramBotClient bot, Update update, long chatId)
    {
        await bot.SendTextMessageAsync(chatId, BotMessages.YourName, parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction2(ITelegramBotClient bot, Update update, long chatId)
    {
        var userInfo = BotDbContext.Instance.UserInfos
            .Where(i => i.UserId == chatId)
            .FirstOrDefault();
        userInfo.Name = update.Message.Text;

        await BotDbContext.Instance.SaveChangesAsync();
        var replyMarkup = new InlineKeyboardMarkup(new []
        {
            new []{InlineKeyboardButton.WithCallbackData("Изменить анкету", BotChatCommands.Change)},
            new []{InlineKeyboardButton.WithCallbackData("Назад \u21a9\ufe0f", BotChatCommands.Start)},
        });
        await bot.SendTextMessageAsync(chatId, BotMessages.Success, parseMode: ParseMode.MarkdownV2, replyMarkup: replyMarkup);
    }

    public ChangeNameScenario()
    {
        this.steps = new List<BotCommandScenarioStep>
        {
            new (StepAction1),
            new (StepAction2),

        }.GetEnumerator();
    }
}