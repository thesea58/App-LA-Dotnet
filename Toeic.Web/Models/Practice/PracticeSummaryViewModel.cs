namespace Toeic.Web.Models.Practice;

public class PracticeSummaryViewModel
{
	public int TotalAnswered { get; set; }
	public int TotalCorrect { get; set; }
	public double AccuracyPercentage => TotalAnswered == 0 ? 0 : Math.Round((double)TotalCorrect / TotalAnswered * 100, 2);
	public IReadOnlyList<PracticePartStatViewModel> PartStats { get; set; } = [];
	public IReadOnlyList<PracticeGrammarTagStatViewModel> GrammarTagStats { get; set; } = [];
}

public class PracticePartStatViewModel
{
	public string Part { get; set; } = string.Empty;
	public int Total { get; set; }
	public int Correct { get; set; }
	public double AccuracyPercentage => Total == 0 ? 0 : Math.Round((double)Correct / Total * 100, 2);
}

public class PracticeGrammarTagStatViewModel
{
	public string GrammarTag { get; set; } = string.Empty;
	public int Total { get; set; }
	public int Incorrect { get; set; }
}
