namespace Toeic.Domain.Entities;

public class Assignment
{
	public int Id { get; set; }
	public int ClassRoomId { get; set; }
	public int MockTestId { get; set; }
	public string Title { get; set; } = string.Empty;
	public DateTime DueDate { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ClassRoom? ClassRoom { get; set; }
	public MockTest? MockTest { get; set; }
}
