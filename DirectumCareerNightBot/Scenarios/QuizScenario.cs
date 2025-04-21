using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using BotCommon.Scenarios;
using HalloweenDirectumBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DirectumCareerNightBot.Scenarios;

public class QuizScenario : AutoStepBotCommandScenario
{
    public override Guid Id { get; } = new Guid("C2181931-E2A2-409D-9E9B-220177C40556");
    public override string ScenarioCommand { get; }
    private async Task StepAction1(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);
        var userQuestions = BotDbContext.Instance.UserQuestions.Where(q => q.UserId == botUser.Id);
        if (userQuestions.Any())
            BotDbContext.Instance.UserQuestions.RemoveRange(userQuestions);
        
        var question = GetQuizQuestion(BotDbContext.Instance, new Random().Next(1, 5));

        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, question, null));
        await BotDbContext.Instance.SaveChangesAsync();
        
        var questionChoices = GetQuestionChoices(BotDbContext.Instance, question);
        var replyMarkup = GetChoicesReplyMarkup(questionChoices);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId, 
        $"{BotMessages.QuizStartMessage}\n{question.Text}", 
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2);
    }

    private async Task StepAction2(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);

        var userChoiceId = BotDbContext.Instance.Choices
            .Single(c => c.Id == int.Parse(update.CallbackQuery.Data));
        var lastUserQuestion = BotDbContext.Instance.UserQuestions
            .Where(c => c.User.Id == botUser.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();
        var quizQuestion = BotDbContext.Instance.Questions.SingleOrDefault(q => lastUserQuestion.QuestionId == q.Id);
        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, quizQuestion, 
            quizQuestion.CorrectChoiceId == userChoiceId.Id));

        var question = GetQuizQuestion(BotDbContext.Instance, new Random().Next(5, 9));


        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, question, null));
        await BotDbContext.Instance.SaveChangesAsync();

        var questionChoices = GetQuestionChoices(BotDbContext.Instance, question);
        var replyMarkup = GetChoicesReplyMarkup(questionChoices);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId,
            text: question.Text,
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction3(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);

        var userChoiceId = BotDbContext.Instance.Choices
            .Single(c => c.Id == int.Parse(update.CallbackQuery.Data));
        var lastUserQuestion = BotDbContext.Instance.UserQuestions
            .Where(c => c.User.Id == botUser.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();
        var quizQuestion = BotDbContext.Instance.Questions.SingleOrDefault(q => lastUserQuestion.QuestionId == q.Id);
        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, quizQuestion, 
            quizQuestion.CorrectChoiceId == userChoiceId.Id));

        var question = GetQuizQuestion(BotDbContext.Instance, new Random().Next(9, 13));


        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, question, null));
        await BotDbContext.Instance.SaveChangesAsync();

        var questionChoices = GetQuestionChoices(BotDbContext.Instance, question);
        var replyMarkup = GetChoicesReplyMarkup(questionChoices);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId,
            text: question.Text,
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction4(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);

        var userChoiceId = BotDbContext.Instance.Choices
            .Single(c => c.Id == int.Parse(update.CallbackQuery.Data));
        var lastUserQuestion = BotDbContext.Instance.UserQuestions
            .Where(c => c.User.Id == botUser.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();
        var quizQuestion = BotDbContext.Instance.Questions.SingleOrDefault(q => lastUserQuestion.QuestionId == q.Id);
        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, quizQuestion, 
            quizQuestion.CorrectChoiceId == userChoiceId.Id));

        var question = GetQuizQuestion(BotDbContext.Instance, new Random().Next(13, 17));


        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, question, null));
        await BotDbContext.Instance.SaveChangesAsync();

        var questionChoices = GetQuestionChoices(BotDbContext.Instance, question);
        var replyMarkup = GetChoicesReplyMarkup(questionChoices);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId,
            text: question.Text,
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction5(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);

        var userChoiceId = BotDbContext.Instance.Choices
            .Single(c => c.Id == int.Parse(update.CallbackQuery.Data));
        var lastUserQuestion = BotDbContext.Instance.UserQuestions
            .Where(c => c.User.Id == botUser.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();
        var quizQuestion = BotDbContext.Instance.Questions.SingleOrDefault(q => lastUserQuestion.QuestionId == q.Id);
        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, quizQuestion, 
            quizQuestion.CorrectChoiceId == userChoiceId.Id));

        var question = GetQuizQuestion(BotDbContext.Instance, new Random().Next(17, 21));


        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, question, null));
        await BotDbContext.Instance.SaveChangesAsync();

        var questionChoices = GetQuestionChoices(BotDbContext.Instance, question);
        var replyMarkup = GetChoicesReplyMarkup(questionChoices);
        await bot.EditMessageTextAsync(
            chatId,
            update.CallbackQuery.Message.MessageId,
            text: question.Text,
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2);
    }
    private async Task StepAction6(ITelegramBotClient bot, Update update, long chatId)
    {
        var botUser = BotDbContext.Instance.BotUsers.First(u => u.Id == BotHelper.GetUserInfo(update).Id);

        var userChoiceId = BotDbContext.Instance.Choices
            .Single(c => c.Id == int.Parse(update.CallbackQuery.Data));
        var lastUserQuestion = BotDbContext.Instance.UserQuestions
            .Where(c => c.User.Id == botUser.Id)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();
        var quizQuestion = BotDbContext.Instance.Questions.SingleOrDefault(q => lastUserQuestion.QuestionId == q.Id);
        BotDbContext.Instance.UserQuestions.Add(new QuizUserQuestion(botUser, quizQuestion, 
            quizQuestion.CorrectChoiceId == userChoiceId.Id));
        await BotDbContext.Instance.SaveChangesAsync();

        var userQuestions = BotDbContext.Instance.UserQuestions.Where(q => q.UserId == botUser.Id);
        var quizResult = userQuestions.Count(q => q.IsCorrectAnswered.Value);
        
        BotDbContext.Instance.UserQuestions.RemoveRange(userQuestions);
        
        var quizIsDone = quizResult > 0;
        
        var resultMessage = string.Empty;
        var repeatQuizMessage = "Держи эксклюзивный \u26a1[стикерпак](https://t.me/addstickers/directum_pack)\u26a1 от Directum\\!\nМожешь пройти квиз ещё раз, и каждый раз будешь получать этот стикерпак в подарок\\! Это ли не радость? \ud83d\ude09";
        
        if (quizResult == 0)
            resultMessage = "Кажется ты подустал, зарядись и пройди тест ещё раз \ud83d\ude35\u200d\ud83d\udcab";
        else if (quizResult > 0 && quizResult < 5)
            resultMessage = $"Поздравляем\\!\n\n{repeatQuizMessage}";
        else if (quizResult == 5)
            resultMessage = $"Балдею от твоих ответов\ud83e\udee6\\! Ты такой умный Дядька \\(Тётька\\)\\!\n\n{repeatQuizMessage}";
        
        var userResult = BotDbContext.Instance.UserResults.SingleOrDefault(r => r.UserId == botUser.Id);
        await BotDbContext.Instance.SaveChangesAsync();

        var buttons = new List<InlineKeyboardButton[]>
        {
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.MainMenuButton, BotChatCommands.MainMenu) },
            new[] { InlineKeyboardButton.WithCallbackData(BotMessages.Quiz, BotChatCommands.Quiz) }
        };
        var markup = new InlineKeyboardMarkup(buttons);
        await bot.EditMessageTextAsync(
            chatId, 
            update.CallbackQuery.Message.MessageId,
            $"Твой результат {quizResult} из 5\\.\n{resultMessage}", 
            replyMarkup: markup,
            parseMode: ParseMode.MarkdownV2);
    }

    private static QuizQuestion GetQuizQuestion(BotDbContext botDbContext, int questionId)
    {
        return botDbContext.Questions.First(q => q.Id == questionId);
    }

    private static List<QuizPossibleAnswer> GetQuestionChoices(BotDbContext botDbContext, QuizQuestion question)
    {
        return botDbContext.Choices.Where(c => c.Question.Id == question.Id).ToList();
    }

    private static InlineKeyboardMarkup GetChoicesReplyMarkup(List<QuizPossibleAnswer> questionChoices)
    {
        var choicesKeyboard = new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(questionChoices[0].Text, questionChoices[0].Id.ToString()),
                InlineKeyboardButton.WithCallbackData(questionChoices[1].Text, questionChoices[1].Id.ToString()),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(questionChoices[2].Text, questionChoices[2].Id.ToString()),
                InlineKeyboardButton.WithCallbackData(questionChoices[3].Text, questionChoices[3].Id.ToString()),
            },
        };
        var replyMarkup = new InlineKeyboardMarkup(choicesKeyboard);
        return replyMarkup;
    }

    public QuizScenario()
    {
        this.steps = new List<BotCommandScenarioStep>
        {
            new (StepAction1),
            new (StepAction2),
            new (StepAction3),
            new (StepAction4),
            new (StepAction5),
            new (StepAction6),

        }.GetEnumerator();
    }
}