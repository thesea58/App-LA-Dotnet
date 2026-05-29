using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Toeic.Domain.Entities;
using Toeic.Domain.Enums;
using Toeic.Infrastructure.Identity;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Admin;

namespace Toeic.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
	private readonly AppDbContext _dbContext;

	public AdminController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public IActionResult Index()
	{
		return View();
	}

	[HttpGet("/Admin/Questions")]
	public async Task<IActionResult> Questions()
	{
		var questions = await _dbContext.Questions
			.AsNoTracking()
			.OrderBy(q => q.Id)
			.ToListAsync();

		return View(questions);
	}

	[HttpGet("/Admin/Questions/Details/{id:int}")]
	public async Task<IActionResult> QuestionDetails(int id)
	{
		var question = await _dbContext.Questions
			.Include(q => q.AnswerOptions)
			.AsNoTracking()
			.FirstOrDefaultAsync(q => q.Id == id);

		if (question is null)
		{
			return NotFound();
		}

		return View(question);
	}

	[HttpGet("/Admin/Questions/Create")]
	public IActionResult CreateQuestion()
	{
		return View(new QuestionFormViewModel());
	}

	[HttpPost("/Admin/Questions/Create")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> CreateQuestion(QuestionFormViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var question = new Question
		{
			Part = model.Part!.Value,
			Skill = model.Skill!.Value,
			QuestionText = model.QuestionText,
			Explanation = model.Explanation,
			Difficulty = model.Difficulty,
			Topic = model.Topic,
			GrammarTag = model.GrammarTag,
			CreatedAt = DateTime.UtcNow
		};

		await _dbContext.Questions.AddAsync(question);
		await _dbContext.SaveChangesAsync();
		return RedirectToAction(nameof(Questions));
	}

	[HttpGet("/Admin/Questions/Edit/{id:int}")]
	public async Task<IActionResult> EditQuestion(int id)
	{
		var question = await _dbContext.Questions.FindAsync(id);
		if (question is null)
		{
			return NotFound();
		}

		var model = new QuestionFormViewModel
		{
			Id = question.Id,
			Part = question.Part,
			Skill = question.Skill,
			QuestionText = question.QuestionText,
			Explanation = question.Explanation,
			Difficulty = question.Difficulty,
			Topic = question.Topic,
			GrammarTag = question.GrammarTag
		};

		return View(model);
	}

	[HttpPost("/Admin/Questions/Edit/{id:int}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EditQuestion(int id, QuestionFormViewModel model)
	{
		if (id != model.Id)
		{
			return BadRequest();
		}

		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var question = await _dbContext.Questions.FindAsync(id);
		if (question is null)
		{
			return NotFound();
		}

		question.Part = model.Part!.Value;
		question.Skill = model.Skill!.Value;
		question.QuestionText = model.QuestionText;
		question.Explanation = model.Explanation;
		question.Difficulty = model.Difficulty;
		question.Topic = model.Topic;
		question.GrammarTag = model.GrammarTag;

		await _dbContext.SaveChangesAsync();
		return RedirectToAction(nameof(Questions));
	}

	[HttpGet("/Admin/Questions/Delete/{id:int}")]
	public async Task<IActionResult> DeleteQuestion(int id)
	{
		var question = await _dbContext.Questions
			.AsNoTracking()
			.FirstOrDefaultAsync(q => q.Id == id);

		if (question is null)
		{
			return NotFound();
		}

		return View(question);
	}

	[HttpPost("/Admin/Questions/Delete/{id:int}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteQuestionConfirmed(int id)
	{
		var question = await _dbContext.Questions.FindAsync(id);
		if (question is null)
		{
			return NotFound();
		}

		_dbContext.Questions.Remove(question);
		await _dbContext.SaveChangesAsync();
		return RedirectToAction(nameof(Questions));
	}

	[HttpGet("/Admin/QuestionImport")]
	public IActionResult QuestionImport()
	{
		return View(new QuestionImportViewModel());
	}

	[HttpPost("/Admin/QuestionImport")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> QuestionImport(QuestionImportViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		List<QuestionImportItemDto>? importItems;
		try
		{
			importItems = JsonSerializer.Deserialize<List<QuestionImportItemDto>>(
				model.JsonPayload,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
		}
		catch (JsonException)
		{
			ModelState.AddModelError(string.Empty, "Invalid JSON format.");
			return View(model);
		}

		if (importItems is null || importItems.Count == 0)
		{
			ModelState.AddModelError(string.Empty, "JSON must contain at least one question.");
			return View(model);
		}

		var questionsToAdd = new List<Question>();
		for (var i = 0; i < importItems.Count; i++)
		{
			var item = importItems[i];
			if (string.IsNullOrWhiteSpace(item.QuestionText))
			{
				ModelState.AddModelError(string.Empty, $"Item #{i + 1}: QuestionText is required.");
				continue;
			}

			if (item.Options is null || item.Options.Count < 2)
			{
				ModelState.AddModelError(string.Empty, $"Item #{i + 1}: at least 2 options are required.");
				continue;
			}

			if (item.Options.Count(x => x.IsCorrect) != 1)
			{
				ModelState.AddModelError(string.Empty, $"Item #{i + 1}: exactly 1 correct option is required.");
				continue;
			}

			if (!TryParsePart(item.Part, out var part) || !TryParseSkill(item.Skill, out var skill))
			{
				ModelState.AddModelError(string.Empty, $"Item #{i + 1}: invalid Part or Skill.");
				continue;
			}

			var question = new Question
			{
				QuestionText = item.QuestionText.Trim(),
				Part = part,
				Skill = skill,
				Explanation = item.Explanation ?? string.Empty,
				Difficulty = item.Difficulty,
				Topic = item.Topic ?? string.Empty,
				GrammarTag = item.GrammarTag ?? string.Empty,
				CreatedAt = DateTime.UtcNow
			};

			foreach (var option in item.Options)
			{
				question.AnswerOptions.Add(new AnswerOption
				{
					Label = option.Label,
					Text = option.Text,
					IsCorrect = option.IsCorrect
				});
			}

			questionsToAdd.Add(question);
		}

		if (!ModelState.IsValid)
		{
			return View(model);
		}

		await _dbContext.Questions.AddRangeAsync(questionsToAdd);
		await _dbContext.SaveChangesAsync();

		model.ResultMessage = $"Imported {questionsToAdd.Count} questions successfully.";
		model.JsonPayload = string.Empty;
		return View(model);
	}

	private static bool TryParsePart(string value, out ToeicPart part)
	{
		if (Enum.TryParse<ToeicPart>(value, true, out part))
		{
			return true;
		}

		if (int.TryParse(value, out var partNumber) && partNumber >= 1 && partNumber <= 7)
		{
			part = (ToeicPart)partNumber;
			return true;
		}

		part = default;
		return false;
	}

	private static bool TryParseSkill(string value, out ToeicSkill skill)
	{
		if (Enum.TryParse<ToeicSkill>(value, true, out skill))
		{
			return true;
		}

		if (int.TryParse(value, out var skillNumber) && Enum.IsDefined(typeof(ToeicSkill), skillNumber))
		{
			skill = (ToeicSkill)skillNumber;
			return true;
		}

		skill = default;
		return false;
	}
}
