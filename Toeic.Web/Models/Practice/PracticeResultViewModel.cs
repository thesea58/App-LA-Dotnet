using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Practice;

public class PracticeResultViewModel
{
	public Question Question { get; set; } = null!;
	public int SelectedAnswerOptionId { get; set; }
	public int CorrectAnswerOptionId { get; set; }
	public bool IsCorrect => SelectedAnswerOptionId == CorrectAnswerOptionId;
}
