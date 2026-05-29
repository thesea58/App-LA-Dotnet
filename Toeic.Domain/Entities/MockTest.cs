namespace Toeic.Domain.Entities;

public class MockTest
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int DurationMinutes { get; set; }

	public ICollection<MockTestQuestion> Questions { get; set; } = new List<MockTestQuestion>();
}
