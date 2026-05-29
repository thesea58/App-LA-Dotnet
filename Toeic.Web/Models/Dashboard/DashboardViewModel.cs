namespace Toeic.Web.Models.Dashboard;

public class DashboardViewModel
{
	public int TotalPracticeQuestions { get; set; }
	public double OverallAccuracy { get; set; }
	public IReadOnlyList<DashboardPartStatViewModel> PartStats { get; set; } = [];
	public int MockTestsCompleted { get; set; }
	public int? LatestMockTestScore { get; set; }
	public string WeakestGrammarTag { get; set; } = "N/A";
}

public class DashboardPartStatViewModel
{
	public string Part { get; set; } = string.Empty;
	public double Accuracy { get; set; }
}
