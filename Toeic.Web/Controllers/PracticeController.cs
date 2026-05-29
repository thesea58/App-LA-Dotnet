using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Domain.Enums;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Practice;

namespace Toeic.Web.Controllers;

[Authorize]
public class PracticeController : Controller
{
	private readonly AppDbContext _dbContext;

	public PracticeController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public IActionResult Index()
	{
		return View();
	}

	[HttpGet("/Practice/Part/{partNumber:int}")]
	public async Task<IActionResult> Part(int partNumber)
	{
		if (partNumber < 1 || partNumber > 7)
		{
			return NotFound();
		}

		var part = (ToeicPart)partNumber;
		var questions = await _dbContext.Questions
			.AsNoTracking()
			.Where(q => q.Part == part)
			.OrderBy(q => q.Id)
			.ToListAsync();

		ViewData["PartNumber"] = partNumber;
		return View(questions);
	}

	[HttpGet("/Practice/Question/{id:int}")]
	public async Task<IActionResult> Question(int id)
	{
		var question = await _dbContext.Questions
			.Include(q => q.AnswerOptions)
			.AsNoTracking()
			.FirstOrDefaultAsync(q => q.Id == id);

		if (question is null)
		{
			return NotFound();
		}

		var vm = new PracticeQuestionViewModel { Question = question };
		return View(vm);
	}

	[HttpPost("/Practice/Submit")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Submit(int questionId, int selectedAnswerOptionId)
	{
		var question = await _dbContext.Questions
			.Include(q => q.AnswerOptions)
			.AsNoTracking()
			.FirstOrDefaultAsync(q => q.Id == questionId);

		if (question is null)
		{
			return NotFound();
		}

		var correctOption = question.AnswerOptions.FirstOrDefault(o => o.IsCorrect);
		if (correctOption is null)
		{
			return BadRequest("Question does not have a correct answer.");
		}

		var vm = new PracticeResultViewModel
		{
			Question = question,
			SelectedAnswerOptionId = selectedAnswerOptionId,
			CorrectAnswerOptionId = correctOption.Id
		};

		return View("QuestionResult", vm);
	}
}
