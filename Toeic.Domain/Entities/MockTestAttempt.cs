namespace Toeic.Domain.Entities;

public class MockTestAttempt
{
	public int Id { get; set; }
	public string UserId { get; set; } = string.Empty;
	public int MockTestId { get; set; }
	public DateTime StartedAt { get; set; } = DateTime.UtcNow;
	public DateTime? SubmittedAt { get; set; }
	public int Score { get; set; }

	public MockTest? MockTest { get; set; }
	public ICollection<MockTestAnswer> Answers { get; set; } = new List<MockTestAnswer>();
}
