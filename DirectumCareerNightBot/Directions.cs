using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot;

public static class Directions
{
    public static IEnumerable<KeyboardButton[]> GetDirectionsKeyBoard()
    {
        return new List<KeyboardButton[]>
        {
            new[] { new KeyboardButton(BotMessages.Programming) },
            new[] { new KeyboardButton(BotMessages.Testing) },
            new[] { new KeyboardButton(BotMessages.SailsAndMarketing) },
            new[] { new KeyboardButton(BotMessages.Support) },
            new[] { new KeyboardButton(BotMessages.Analitycs) },
            new[] { new KeyboardButton(BotMessages.TechDoc) },
        };
    }

    public static List<string> AllDirections = new()
    {
        BotMessages.Programming,
        BotMessages.Testing,
        BotMessages.SailsAndMarketing,
        BotMessages.Support,
        BotMessages.Analitycs,
        BotMessages.TechDoc
    };

    public static string GetDirectionAlias(string direction)
    {
        if (direction == BotMessages.Programming)
            return "Разработка";
        if (direction == BotMessages.Testing)
            return "Тестирование";
        if (direction == BotMessages.SailsAndMarketing)
            return "Маркетинг";
        if (direction == BotMessages.Support)
            return "Системные инженеры";
        if (direction == BotMessages.Analitycs)
            return "Аналитика";
        if (direction == BotMessages.TechDoc)
            return "Писатели";
        return "Есть контакт";
    }
}