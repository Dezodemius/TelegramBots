using System;
using System.Threading;
using BotCommon.Broadcast;
using Telegram.Bot;

namespace DirectumCareerNightBot;

public class ScheduledMessageSender
{
  private readonly ITelegramBotClient _botClient;
  private readonly DateTime _targetTime;
  private Timer _timer;
  private readonly string _targetMessage;

  public ScheduledMessageSender(ITelegramBotClient botClient, DateTime targetTime, string targetMessage)
  {
    _botClient = botClient;
    _targetTime = targetTime;
    _targetMessage = targetMessage;
  }

  public void Start()
  {
    _timer = new Timer(CheckTime, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
  }

  private void CheckTime(object state)
  {
    if (Math.Abs((DateTime.Now - _targetTime).TotalMinutes) < 1)
    {
      BroadcastMessageSender.BroadcastMessage(_botClient, BotDbContext.Instance.BotUsers, _targetMessage);
            
      Console.WriteLine($"BROADCAST: Сообщение отправлено в {DateTime.Now}");
      this.Stop();
    }
  }

  public void Stop()
  {
    _timer?.Dispose();
  }
}