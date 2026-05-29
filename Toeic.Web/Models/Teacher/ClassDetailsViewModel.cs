using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Teacher;

public class ClassDetailsViewModel
{
	public ClassRoom ClassRoom { get; set; } = null!;
	public IReadOnlyList<ClassStudentViewModel> Students { get; set; } = [];
}

public class ClassStudentViewModel
{
	public string StudentId { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime EnrolledAt { get; set; }
}
