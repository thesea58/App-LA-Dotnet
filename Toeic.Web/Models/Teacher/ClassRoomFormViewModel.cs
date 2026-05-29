using System.ComponentModel.DataAnnotations;

namespace Toeic.Web.Models.Teacher;

public class ClassRoomFormViewModel
{
	[Required]
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
}
