namespace Toeic.Domain.Entities;

public class PracticeAttempt
{
	public int Id { get; set; }
	public string UserId { get; set; } = string.Empty;
	public int QuestionId { get; set; }
	public int SelectedAnswerOptionId { get; set; }
	public bool IsCorrect { get; set; }
	public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

	public Question? Question { get; set; }
}
