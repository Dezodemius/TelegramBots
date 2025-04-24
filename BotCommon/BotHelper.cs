using BotCommon.Repository;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotCommon;

public static class BotHelper
{
    public static User GetUserInfo(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message.From,
            UpdateType.CallbackQuery => update.CallbackQuery.From,
            _ => default
        };
    }
    
    public static string GetUsername(Update update)
    {
        return GetUsername(GetUserInfo(update));
    }

    public static string GetUsername(User user)
    {
        return string.IsNullOrEmpty(user.Username)
            ? $"{user.FirstName} {user.LastName}"
            : user.Username;
    }
    
    public static string GetUsername(BotUser user)
    {
        return string.IsNullOrEmpty(user.Username)
            ? $"{user.FirstName} {user.LastName}"
            : user.Username;
    }
    
    public static string GetMessage(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message.Text,
            UpdateType.CallbackQuery => update.CallbackQuery.Data,
            _ => null
        };
    }
}