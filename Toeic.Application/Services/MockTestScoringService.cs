using Toeic.Domain.Entities;

namespace Toeic.Application.Services;

public class MockTestScoringService
{
	public int CalculateScore(IEnumerable<MockTestAnswer> answers)
	{
		return answers.Count(a => a.IsCorrect);
	}
}
