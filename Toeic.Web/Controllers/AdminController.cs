using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Domain.Entities;
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
}
