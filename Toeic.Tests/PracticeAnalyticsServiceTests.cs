using Toeic.Application.Services;
using Toeic.Domain.Entities;
using Toeic.Domain.Enums;

namespace Toeic.Tests;

public class PracticeAnalyticsServiceTests
{
	[Fact]
	public void CalculateAccuracyByPart_Returns_Expected_Values()
	{
		var service = new PracticeAnalyticsService();
		var attempts = new List<PracticeAttempt>
		{
			new() { IsCorrect = true, Question = new Question { Part = ToeicPart.Part5 } },
			new() { IsCorrect = false, Question = new Question { Part = ToeicPart.Part5 } },
			new() { IsCorrect = true, Question = new Question { Part = ToeicPart.Part6 } }
		};

		var accuracyByPart = service.CalculateAccuracyByPart(attempts);

		Assert.Equal(50, accuracyByPart["Part5"]);
		Assert.Equal(100, accuracyByPart["Part6"]);
	}

	[Fact]
	public void GetWeakestGrammarTag_Returns_Tag_With_Most_Wrong_Attempts()
	{
		var service = new PracticeAnalyticsService();
		var attempts = new List<PracticeAttempt>
		{
			new() { IsCorrect = false, Question = new Question { GrammarTag = "Verb tense" } },
			new() { IsCorrect = false, Question = new Question { GrammarTag = "Verb tense" } },
			new() { IsCorrect = false, Question = new Question { GrammarTag = "Preposition" } },
			new() { IsCorrect = true, Question = new Question { GrammarTag = "Part of speech" } }
		};

		var weakest = service.GetWeakestGrammarTag(attempts);

		Assert.Equal("Verb tense", weakest);
	}
}
