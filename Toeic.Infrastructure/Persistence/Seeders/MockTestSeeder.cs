using Toeic.Domain.Entities;
using Toeic.Domain.Enums;

namespace Toeic.Infrastructure.Persistence.Seeders;

public static class MockTestSeeder
{
	public static async Task SeedMiniTestAsync(AppDbContext context)
	{
		if (context.MockTests.Any())
		{
			return;
		}

		await EnsureAtLeastTwentyQuestionsAsync(context);

		var questionIds = context.Questions
			.AsQueryable()
			.OrderBy(q => q.Id)
			.Take(20)
			.Select(q => q.Id)
			.ToList();

		if (questionIds.Count < 20)
		{
			return;
		}

		var miniTest = new MockTest
		{
			Title = "TOEIC Mini Test 01",
			Description = "Mini mock test with 20 questions.",
			DurationMinutes = 25,
			Questions = questionIds.Select((id, index) => new MockTestQuestion
			{
				QuestionId = id,
				OrderNo = index + 1
			}).ToList()
		};

		await context.MockTests.AddAsync(miniTest);
		await context.SaveChangesAsync();
	}

	private static async Task EnsureAtLeastTwentyQuestionsAsync(AppDbContext context)
	{
		var currentCount = context.Questions.Count();
		if (currentCount >= 20)
		{
			return;
		}

		for (var i = currentCount + 1; i <= 20; i++)
		{
			var question = new Question
			{
				Part = ToeicPart.Part5,
				Skill = ToeicSkill.Reading,
				QuestionText = $"For question set {i}, please choose the most suitable word to complete the sentence.",
				Explanation = "Đây là câu bổ sung để đủ số lượng cho mini test 20 câu.",
				Difficulty = 2,
				Topic = "Office",
				GrammarTag = "Part of speech",
				CreatedAt = DateTime.UtcNow
			};

			question.AnswerOptions.Add(new AnswerOption { Label = "A", Text = "efficient", IsCorrect = true });
			question.AnswerOptions.Add(new AnswerOption { Label = "B", Text = "efficiency", IsCorrect = false });
			question.AnswerOptions.Add(new AnswerOption { Label = "C", Text = "efficiently", IsCorrect = false });
			question.AnswerOptions.Add(new AnswerOption { Label = "D", Text = "efficiencies", IsCorrect = false });

			await context.Questions.AddAsync(question);
		}

		await context.SaveChangesAsync();
	}
}
