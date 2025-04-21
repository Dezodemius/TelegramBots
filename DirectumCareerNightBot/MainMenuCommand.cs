using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot;

public static class MainMenuCommand
{
    public static IEnumerable<InlineKeyboardButton[]> GetMainMenuButtons()
    {
        return new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.Student, BotChatCommands.Student) },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.NotStudent, BotChatCommands.NotStudent) },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.Quiz, BotChatCommands.Quiz) }
        };
    }
    public static InlineKeyboardMarkup GetMainMenuInlineMarkup()
    {
        return new InlineKeyboardMarkup(GetMainMenuButtons());
    }
}