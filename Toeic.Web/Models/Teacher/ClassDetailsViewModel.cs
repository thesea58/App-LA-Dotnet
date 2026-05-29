using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Teacher;

public class ClassDetailsViewModel
{
	public ClassRoom ClassRoom { get; set; } = null!;
	public IReadOnlyList<ClassStudentViewModel> Students { get; set; } = [];
	public IReadOnlyList<ClassAssignmentViewModel> Assignments { get; set; } = [];
	public IReadOnlyList<MockTestOptionViewModel> MockTests { get; set; } = [];
}

public class ClassStudentViewModel
{
	public string StudentId { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime EnrolledAt { get; set; }
}

public class ClassAssignmentViewModel
{
	public string Title { get; set; } = string.Empty;
	public string MockTestTitle { get; set; } = string.Empty;
	public DateTime DueDate { get; set; }
}

public class MockTestOptionViewModel
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
}
