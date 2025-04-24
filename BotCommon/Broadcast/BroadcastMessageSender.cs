using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using BotCommon.Repository;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace BotCommon.Broadcast;

public static class BroadcastMessageSender
{
  private static readonly TimeSpan BroadcastTimeout = TimeSpan.FromSeconds(5);

  public static async void BroadcastMessage(
    ITelegramBotClient botClient, 
    IEnumerable<BotUser> users, 
    string message)
  {
    LogManager.GetCurrentClassLogger().Debug("BROADCAST START");

    var userList = users.ToList();
    for (var i = 0; i < userList.Count; i++)
    {
      var botUser = userList[i];
      try
      {
        await botClient.SendTextMessageAsync(botUser.Id, message, parseMode: ParseMode.MarkdownV2);

        var broadcastMessageUser = new BroadcastMessageUser(botUser.Id, DateTime.Now, true, false);
        BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Add(broadcastMessageUser);
        await BroadcastMessageDbContext.Instance.SaveChangesAsync();

        LogManager.GetCurrentClassLogger().Debug($"{i + 1}/{userList.Count} Message sent to: {botUser.Id} ({BotHelper.GetUsername(botUser)})");
      }
      catch (ApiRequestException e) when (e.ErrorCode == (int)HttpStatusCode.Forbidden)
      {
        if (BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Any(u => u.UserId == botUser.Id))
        {
          var broadcastMessageUser = BroadcastMessageDbContext.Instance.BroadcastMessageUsers
            .First(u => u.UserId == botUser.Id);
          broadcastMessageUser.IsBotBlocked = true;
          await BroadcastMessageDbContext.Instance.SaveChangesAsync();
        }
        else
        {
          var broadcastMessageUser = new BroadcastMessageUser(botUser.Id, DateTime.Now, false, true);
          BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Add(broadcastMessageUser);
          await BroadcastMessageDbContext.Instance.SaveChangesAsync();
        }

        LogManager.GetCurrentClassLogger().Warn($"{i + 1}/{userList.Count} User {botUser.Id} blocked bot");
      }
      catch (ApiRequestException e) when (e.ErrorCode == (int)HttpStatusCode.TooManyRequests)
      {
        LogManager.GetCurrentClassLogger().Warn("TOO MANY REQUEST. BROADCAST GOING TO PAUSE!");
        LogManager.GetCurrentClassLogger().Warn(e);
        i--;
        Thread.Sleep(TimeSpan.FromMinutes(2));
      }
      catch (Exception e)
      {
        LogManager.GetCurrentClassLogger().Error(e);
      }
      if (i % 10 == 0)
        Thread.Sleep(BroadcastTimeout);
    }
    
    LogManager.GetCurrentClassLogger().Debug("BROADCAST COMPLETED");
  }
  
  public static async void BroadcastMessageWithPhoto(
    ITelegramBotClient botClient, 
    IEnumerable<BotUser> users, 
    string message,
    string photoPath)
  {
    LogManager.GetCurrentClassLogger().Debug("BROADCAST START");

    
    var userList = users.ToList();
    for (var i = 0; i < userList.Count; i++)
    {
      var botUser = userList[i];
      try
      {
        var photo = InputFile.FromStream(File.OpenRead(photoPath));
        await botClient.SendPhotoAsync(botUser.Id, photo, caption: message, parseMode: ParseMode.MarkdownV2);

        var broadcastMessageUser = new BroadcastMessageUser(botUser.Id, DateTime.Now, true, false);
        BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Add(broadcastMessageUser);
        await BroadcastMessageDbContext.Instance.SaveChangesAsync();

        LogManager.GetCurrentClassLogger().Debug($"{i + 1}/{userList.Count} Message sent to: {botUser.Id}");
      }
      catch (ApiRequestException e) when (e.ErrorCode == (int)HttpStatusCode.Forbidden)
      {
        if (BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Any(u => u.UserId == botUser.Id))
        {
          var broadcastMessageUser = BroadcastMessageDbContext.Instance.BroadcastMessageUsers
            .First(u => u.UserId == botUser.Id);
          broadcastMessageUser.IsBotBlocked = true;
          await BroadcastMessageDbContext.Instance.SaveChangesAsync();
        }
        else
        {
          var broadcastMessageUser = new BroadcastMessageUser(botUser.Id, DateTime.Now, false, true);
          BroadcastMessageDbContext.Instance.BroadcastMessageUsers.Add(broadcastMessageUser);
          await BroadcastMessageDbContext.Instance.SaveChangesAsync();
        }

        LogManager.GetCurrentClassLogger().Warn($"{i + 1}/{userList.Count} User {botUser.Id} blocked bot");
      }
      catch (ApiRequestException e) when (e.ErrorCode == (int)HttpStatusCode.TooManyRequests)
      {
        LogManager.GetCurrentClassLogger().Warn("TOO MANY REQUEST. BROADCAST GOING TO PAUSE!");
        LogManager.GetCurrentClassLogger().Warn(e);
        i--;
        Thread.Sleep(TimeSpan.FromMinutes(2));
      }
      catch (Exception e)
      {
        LogManager.GetCurrentClassLogger().Error(e);
      }
      if (i % 10 == 0)
        Thread.Sleep(BroadcastTimeout);
    }
    
    LogManager.GetCurrentClassLogger().Debug("BROADCAST COMPLETED");
  }
}