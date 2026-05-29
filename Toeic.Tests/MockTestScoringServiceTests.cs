using Toeic.Application.Services;
using Toeic.Domain.Entities;

namespace Toeic.Tests;

public class MockTestScoringServiceTests
{
	[Fact]
	public void CalculateScore_Returns_Number_Of_Correct_Answers()
	{
		var service = new MockTestScoringService();
		var answers = new List<MockTestAnswer>
		{
			new() { IsCorrect = true },
			new() { IsCorrect = false },
			new() { IsCorrect = true }
		};

		var score = service.CalculateScore(answers);

		Assert.Equal(2, score);
	}
}
