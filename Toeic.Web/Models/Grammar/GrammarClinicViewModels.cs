using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Grammar;

public class GrammarClinicGroupViewModel
{
	public string GrammarTag { get; set; } = string.Empty;
	public int WrongCount { get; set; }
}

public class GrammarClinicTagDetailViewModel
{
	public string GrammarTag { get; set; } = string.Empty;
	public IReadOnlyList<PracticeAttempt> WrongAttempts { get; set; } = [];
}
