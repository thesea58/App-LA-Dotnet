using System.ComponentModel.DataAnnotations;
using Toeic.Domain.Enums;

namespace Toeic.Web.Models.Admin;

public class QuestionFormViewModel
{
	public int? Id { get; set; }

	[Required]
	public ToeicPart? Part { get; set; }

	[Required]
	public ToeicSkill? Skill { get; set; }

	[Required]
	public string QuestionText { get; set; } = string.Empty;

	public string Explanation { get; set; } = string.Empty;

	public int Difficulty { get; set; } = 1;

	public string Topic { get; set; } = string.Empty;

	public string GrammarTag { get; set; } = string.Empty;
}
