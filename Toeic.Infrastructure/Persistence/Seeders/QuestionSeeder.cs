using Toeic.Domain.Entities;
using Toeic.Domain.Enums;

namespace Toeic.Infrastructure.Persistence.Seeders;

public static class QuestionSeeder
{
	public static async Task SeedPart5QuestionsAsync(AppDbContext context)
	{
		if (context.Questions.Any())
		{
			return;
		}

		var questions = new List<Question>
		{
			CreateQuestion(
				"The finance manager asked Ms. Lan to ______ the quarterly expense report before noon.",
				"Finance",
				"Part of speech",
				"Ở đây cần động từ nguyên mẫu sau 'to', nên 'review' là đáp án đúng.",
				("A", "review", true),
				("B", "reviewer", false),
				("C", "reviewing", false),
				("D", "reviewed", false)),

			CreateQuestion(
				"Please submit your travel receipts ______ Friday so accounting can process reimbursement.",
				"Travel",
				"Preposition",
				"Dùng 'by Friday' để chỉ hạn chót phải nộp trước hoặc đúng thứ Sáu.",
				("A", "by", true),
				("B", "with", false),
				("C", "among", false),
				("D", "during", false)),

			CreateQuestion(
				"Our department ______ a new scheduling system last month to reduce delays.",
				"Office",
				"Verb tense",
				"'Last month' là mốc thời gian quá khứ, nên dùng thì quá khứ đơn 'implemented'.",
				("A", "implements", false),
				("B", "implemented", true),
				("C", "will implement", false),
				("D", "is implementing", false)),

			CreateQuestion(
				"The conference room is unavailable this afternoon; ______, we will meet in Room 2B.",
				"Meeting",
				"Conjunction",
				"Vế sau là kết quả của việc phòng họp không dùng được, nên dùng 'therefore'.",
				("A", "therefore", true),
				("B", "unless", false),
				("C", "because", false),
				("D", "while", false)),

			CreateQuestion(
				"Mr. Patel will lead the client presentation, and Ms. Hoa will assist ______ during the Q&A session.",
				"Meeting",
				"Part of speech",
				"Sau động từ 'assist' cần tân ngữ, nên phải dùng đại từ tân ngữ 'him'.",
				("A", "he", false),
				("B", "his", false),
				("C", "him", true),
				("D", "himself", false)),

			CreateQuestion(
				"The shipment was delayed ______ severe weather conditions at the port.",
				"Logistics",
				"Preposition",
				"Theo sau là cụm danh từ 'severe weather conditions', nên dùng 'because of'.",
				("A", "because", false),
				("B", "because of", true),
				("C", "despite", false),
				("D", "although", false)),

			CreateQuestion(
				"If the supplier confirms today, we ______ the new inventory tomorrow morning.",
				"Supply Chain",
				"Verb tense",
				"Mệnh đề điều kiện loại 1: If + hiện tại đơn, mệnh đề chính dùng 'will + V'.",
				("A", "received", false),
				("B", "receive", false),
				("C", "will receive", true),
				("D", "have received", false)),

			CreateQuestion(
				"The annual training workshop is open to all employees, ______ new hires.",
				"Recruitment",
				"Part of speech",
				"'Including' dùng để thêm ví dụ thành phần thuộc tập hợp nhân viên.",
				("A", "include", false),
				("B", "includes", false),
				("C", "included", false),
				("D", "including", true)),

			CreateQuestion(
				"The marketing team completed the campaign plan, ______ the design team still needs two more days.",
				"Office",
				"Conjunction",
				"Hai mệnh đề trái nghĩa nhau nên dùng liên từ đối lập 'but'.",
				("A", "but", true),
				("B", "so", false),
				("C", "for", false),
				("D", "or", false)),

			CreateQuestion(
				"Before signing the contract, please read the warranty terms ______.",
				"Legal",
				"Part of speech",
				"Bổ nghĩa cho động từ 'read' cần trạng từ, nên dùng 'carefully'.",
				("A", "careful", false),
				("B", "carefully", true),
				("C", "carefulness", false),
				("D", "care", false))
		};

		await context.Questions.AddRangeAsync(questions);
		await context.SaveChangesAsync();
	}

	private static Question CreateQuestion(
		string questionText,
		string topic,
		string grammarTag,
		string explanation,
		params (string Label, string Text, bool IsCorrect)[] options)
	{
		var question = new Question
		{
			Part = ToeicPart.Part5,
			Skill = ToeicSkill.Reading,
			QuestionText = questionText,
			Explanation = explanation,
			Difficulty = 2,
			Topic = topic,
			GrammarTag = grammarTag,
			CreatedAt = DateTime.UtcNow
		};

		foreach (var option in options)
		{
			question.AnswerOptions.Add(new AnswerOption
			{
				Label = option.Label,
				Text = option.Text,
				IsCorrect = option.IsCorrect
			});
		}

		return question;
	}
}
