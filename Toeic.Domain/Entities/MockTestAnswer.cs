namespace Toeic.Domain.Entities;

public class MockTestAnswer
{
	public int Id { get; set; }
	public int MockTestAttemptId { get; set; }
	public int QuestionId { get; set; }
	public int SelectedAnswerOptionId { get; set; }
	public bool IsCorrect { get; set; }

	public MockTestAttempt? MockTestAttempt { get; set; }
	public Question? Question { get; set; }
}
