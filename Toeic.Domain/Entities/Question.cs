using Toeic.Domain.Enums;

namespace Toeic.Domain.Entities;

public class Question
{
	public int Id { get; set; }
	public ToeicPart Part { get; set; }
	public ToeicSkill Skill { get; set; }
	public string QuestionText { get; set; } = string.Empty;
	public string Explanation { get; set; } = string.Empty;
	public int Difficulty { get; set; }
	public string Topic { get; set; } = string.Empty;
	public string GrammarTag { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}
