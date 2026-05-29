namespace Toeic.Web.Models.Common;

public class TestPlayerViewModel
{
	public string Title { get; set; } = string.Empty;
	public string QuestionText { get; set; } = string.Empty;
	public IReadOnlyList<TestPlayerOptionViewModel> Options { get; set; } = [];
	public int CurrentNumber { get; set; }
	public int TotalQuestions { get; set; }
	public int? SelectedOptionId { get; set; }
	public string FormAction { get; set; } = string.Empty;
	public Dictionary<string, string> HiddenFields { get; set; } = [];
	public string? PreviousUrl { get; set; }
	public string NextCommandValue { get; set; } = "next";
	public string NextButtonLabel { get; set; } = "Next";
}

public class TestPlayerOptionViewModel
{
	public int Id { get; set; }
	public string Label { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
}
