namespace Toeic.Web.Models.Dashboard;

public class DashboardViewModel
{
	public int TotalPracticeQuestions { get; set; }
	public double OverallAccuracy { get; set; }
	public IReadOnlyList<DashboardPartStatViewModel> PartStats { get; set; } = [];
	public int MockTestsCompleted { get; set; }
	public int? LatestMockTestScore { get; set; }
	public string WeakestGrammarTag { get; set; } = "N/A";
	public IReadOnlyList<string> Recommendations { get; set; } = [];
	public IReadOnlyList<StudentAssignmentViewModel> StudentAssignments { get; set; } = [];
}

public class DashboardPartStatViewModel
{
	public string Part { get; set; } = string.Empty;
	public double Accuracy { get; set; }
}

public class StudentAssignmentViewModel
{
	public string ClassName { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string MockTestTitle { get; set; } = string.Empty;
	public DateTime DueDate { get; set; }
}
