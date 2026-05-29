using System.ComponentModel.DataAnnotations;

namespace Toeic.Web.Models.Admin;

public class QuestionImportViewModel
{
	[Required]
	public string JsonPayload { get; set; } = string.Empty;

	public string? ResultMessage { get; set; }
}

public class QuestionImportItemDto
{
	public string QuestionText { get; set; } = string.Empty;
	public string Part { get; set; } = string.Empty;
	public string Skill { get; set; } = string.Empty;
	public string Explanation { get; set; } = string.Empty;
	public int Difficulty { get; set; }
	public string Topic { get; set; } = string.Empty;
	public string GrammarTag { get; set; } = string.Empty;
	public List<QuestionImportOptionDto> Options { get; set; } = [];
}

public class QuestionImportOptionDto
{
	public string Label { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public bool IsCorrect { get; set; }
}
