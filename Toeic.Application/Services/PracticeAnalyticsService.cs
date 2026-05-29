using Toeic.Domain.Entities;

namespace Toeic.Application.Services;

public class PracticeAnalyticsService
{
	public IDictionary<string, double> CalculateAccuracyByPart(IEnumerable<PracticeAttempt> attempts)
	{
		return attempts
			.Where(a => a.Question is not null)
			.GroupBy(a => a.Question!.Part.ToString())
			.ToDictionary(
				g => g.Key,
				g => g.Any() ? Math.Round((double)g.Count(x => x.IsCorrect) / g.Count() * 100, 2) : 0);
	}

	public string GetWeakestGrammarTag(IEnumerable<PracticeAttempt> attempts)
	{
		return attempts
			.Where(a => !a.IsCorrect && a.Question is not null && !string.IsNullOrWhiteSpace(a.Question!.GrammarTag))
			.GroupBy(a => a.Question!.GrammarTag)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key)
			.FirstOrDefault() ?? "N/A";
	}
}
