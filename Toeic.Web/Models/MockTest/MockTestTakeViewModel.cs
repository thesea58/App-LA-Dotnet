using Toeic.Domain.Entities;

namespace Toeic.Web.Models.MockTest;

public class MockTestTakeViewModel
{
	public int AttemptId { get; set; }
	public int OrderNo { get; set; }
	public int TotalQuestions { get; set; }
	public Question Question { get; set; } = null!;
	public int? SelectedAnswerOptionId { get; set; }
}

public class MockTestResultViewModel
{
	public int AttemptId { get; set; }
	public string TestTitle { get; set; } = string.Empty;
	public int CorrectCount { get; set; }
	public int TotalQuestions { get; set; }
	public double AccuracyPercentage => TotalQuestions == 0 ? 0 : Math.Round((double)CorrectCount / TotalQuestions * 100, 2);
	public IReadOnlyList<MockTestWrongItemViewModel> WrongItems { get; set; } = [];
}

public class MockTestWrongItemViewModel
{
	public int QuestionId { get; set; }
	public string QuestionText { get; set; } = string.Empty;
	public string Explanation { get; set; } = string.Empty;
	public string SelectedAnswer { get; set; } = string.Empty;
	public string CorrectAnswer { get; set; } = string.Empty;
}
