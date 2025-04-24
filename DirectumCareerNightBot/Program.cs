using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotCommon;
using BotCommon.Broadcast;
using BotCommon.KeepAlive;
using BotCommon.Repository;
using HalloweenDirectumBot;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace DirectumCareerNightBot;

internal class Program
{
    private static readonly ILogger log = LogManager.GetCurrentClassLogger();

    public static void Main(string[] args)
    {
        var bot = new TelegramBotClient(new BotConfigManager().Config.BotToken);
        PrepareForStartBot(bot);
        StartBot(bot);
        string command;
        do
        {
            command = Console.ReadLine();
        } while (!command.Equals("/exit", StringComparison.InvariantCulture));
        log.Info("Bye bye");
        Environment.Exit(0);
    }

    private static void PrepareForStartBot(ITelegramBotClient bot)
    {
        var botKeepAlive = new BotKeepAlive(bot);
        botKeepAlive.StartKeepAlive();
    }

    private static void StartBot(ITelegramBotClient bot)
    {
        log.Debug("Start Bot");
        var opts = new ReceiverOptions
        {
            AllowedUpdates = new []
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            },
            ThrowPendingUpdates = true
        };
        bot.StartReceiving<BotUpdateHandler>(receiverOptions: opts);
        
       var senderWith1830 = new ScheduledMessageSender(bot, new DateTime(2025, 04, 25, 18, 25, 00), string.Format(BotMessages.BroadcastMessage, "18:30"));
       senderWith1830.Start();
       var senderWith1900 = new ScheduledMessageSender(bot, new DateTime(2025, 04, 25,18, 55, 00), string.Format(BotMessages.BroadcastMessage, "19:00"));
       senderWith1900.Start();   
       var senderWith2000 = new ScheduledMessageSender(bot, new DateTime(2025, 04, 25, 19, 55, 00), string.Format(BotMessages.BroadcastMessage, "20:00"));
       senderWith2000.Start();
    }
} 