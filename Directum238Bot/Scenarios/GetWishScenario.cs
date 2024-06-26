using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotCommon.Scenarios;
using Directum238Bot.Repository;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Directum238Bot.Scenarios;

public class GetWishScenario : AutoStepBotCommandScenario
{
  private readonly string wishDay;
  private readonly UserContentCache cache;
  public override Guid Id => new Guid("3BFA76E9-D083-42D9-97E7-EAC8CE06A41D");
  public override string ScenarioCommand => string.Empty;

  public async Task GetWish(ITelegramBotClient botClient, Update update, long chatId)
  {
    var wish = this.cache.GetRandomContentExceptCurrent(chatId, this.wishDay);
    var markup = new InlineKeyboardMarkup(new []
    {
      InlineKeyboardButton.WithCallbackData(Directum238BotResources.GoStartMenu, BotChatCommand.MainMenu)
    });
    if (wish == null)
      await botClient.SendTextMessageAsync(chatId, Directum238BotResources.NoWishesYet, replyMarkup: markup);
    LogManager.GetCurrentClassLogger().Debug($"Wish content: {wish.Content}. Wish type: {wish.Type}. Wish id: {wish.Id}");
    await botClient.SendTextMessageAsync(chatId, Directum238BotResources.GetWishMessage);
    switch (wish.Type)
    {
      case MessageType.Voice:
      {
        await botClient.SendVoiceAsync(chatId, new InputMedia(wish.Content));
        break;
      }
      case MessageType.VideoNote:
      {
        await botClient.SendVideoNoteAsync(chatId, new InputMedia(wish.Content));
        break;
      }
      case MessageType.Text:
      {
        await botClient.SendTextMessageAsync(chatId, wish.Content);
        break;
      }
    }

    await botClient.SendTextMessageAsync(chatId,
      text: Directum238BotResources.AfterGetWishMessage,
      replyMarkup: markup);
  }

  public GetWishScenario(UserContentCache cache, string wishDay)
  {
    this.cache = cache;
    this.wishDay = wishDay;
    this.steps = new List<BotCommandScenarioStep>
    {
      new (GetWish)
    }.GetEnumerator();
  }
}