using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotCommon.Repository;
using BotCommon.Scenarios;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Directum238Bot.Scenarios;

public class BroadcastScenario : AutoStepBotCommandScenario
{
  private readonly UserDbContext cache;

  public override Guid Id => new Guid("8F2DD0DC-E84B-4319-8277-58222A671FC5");

  public override string ScenarioCommand { get; }

  private async Task RequestMessageToBroadcast(ITelegramBotClient bot, Update update, long userId)
  {
    await bot.SendTextMessageAsync(userId, "Введи сообщение:");
  }

  private async Task BroadcastMessageCheck(ITelegramBotClient bot, Update update, long chatId)
  {
    var keyboard = new InlineKeyboardMarkup(new []
    {
      InlineKeyboardButton.WithCallbackData("Отправить", "Отправить"),
      InlineKeyboardButton.WithCallbackData("Удалить", "Удалить"),
    });
    switch (update.Message.Type)
    {
      case MessageType.Voice:
      {
        await bot.SendVoiceAsync(chatId, new InputOnlineFile(update.Message.Voice.FileId), replyMarkup: keyboard);
        break;
      }
      case MessageType.VideoNote:
      {
        await bot.SendVideoNoteAsync(chatId, new InputOnlineFile(update.Message.VideoNote.FileId), replyMarkup: keyboard);
        break;
      }
      case MessageType.Text:
      {
        await bot.SendTextMessageAsync(chatId, update.Message.Text, replyMarkup: keyboard);
        break;
      }
      case MessageType.Audio:
      {
        await bot.SendAudioAsync(chatId, new InputOnlineFile(update.Message.Text), replyMarkup: keyboard);
        break;
      }
      case MessageType.Video:
      {
        await bot.SendVideoAsync(chatId, new InputOnlineFile(update.Message.Text), replyMarkup: keyboard);
        break;
      }
      case MessageType.Document:
      {
        await bot.SendDocumentAsync(chatId, new InputOnlineFile(update.Message.Text), replyMarkup: keyboard);
        break;
      }
    }
    await bot.DeleteMessageAsync(chatId, update.Message.MessageId);
  }

  private async Task CheckMessage(ITelegramBotClient bot, Update update, long chatId)
  {
    switch (update.CallbackQuery.Data)
    {
      case "Отправить":
      {
        var allUsers = cache.GetAll();
        foreach (var user in allUsers)
        {
          switch (update.CallbackQuery.Message.Type)
          {
            case MessageType.Voice:
            {
              await bot.SendVoiceAsync(user.Id, new InputOnlineFile(update.CallbackQuery.Message.Voice.FileId));
              break;
            }
            case MessageType.VideoNote:
            {
              await bot.SendVideoNoteAsync(user.Id, new InputOnlineFile(update.CallbackQuery.Message.VideoNote.FileId));
              break;
            }
            case MessageType.Text:
            {
              await bot.SendTextMessageAsync(user.Id, update.CallbackQuery.Message.Text);
              break;
            }
          }
        }
        break;
      }
      case "Удалить":
      {
        await bot.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
        break;
      }
    }
  }

  public BroadcastScenario(UserDbContext cache)
  {
    this.cache = cache;
    this.steps = new List<BotCommandScenarioStep>
    {
        new (RequestMessageToBroadcast),
        new (BroadcastMessageCheck),
        new (CheckMessage)
    }.GetEnumerator();
  }
}