namespace Toeic.Domain.Entities;

public class ClassEnrollment
{
	public int Id { get; set; }
	public int ClassRoomId { get; set; }
	public string StudentId { get; set; } = string.Empty;
	public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

	public ClassRoom? ClassRoom { get; set; }
}
