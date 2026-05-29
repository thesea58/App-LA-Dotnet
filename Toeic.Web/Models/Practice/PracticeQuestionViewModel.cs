using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Practice;

public class PracticeQuestionViewModel
{
	public Question Question { get; set; } = null!;
	public int? SelectedAnswerOptionId { get; set; }
}
