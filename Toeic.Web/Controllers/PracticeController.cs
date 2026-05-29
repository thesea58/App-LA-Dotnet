using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Domain.Entities;
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

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!string.IsNullOrWhiteSpace(userId))
		{
			var attempt = new PracticeAttempt
			{
				UserId = userId,
				QuestionId = question.Id,
				SelectedAnswerOptionId = selectedAnswerOptionId,
				IsCorrect = vm.IsCorrect,
				AnsweredAt = DateTime.UtcNow
			};

			await _dbContext.PracticeAttempts.AddAsync(attempt);
			await _dbContext.SaveChangesAsync();
		}

		return View("QuestionResult", vm);
	}

	[HttpGet("/Practice/Result")]
	public async Task<IActionResult> Result()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var attempts = await _dbContext.PracticeAttempts
			.Include(a => a.Question)
			.AsNoTracking()
			.Where(a => a.UserId == userId)
			.ToListAsync();

		var totalAnswered = attempts.Count;
		var totalCorrect = attempts.Count(a => a.IsCorrect);

		var partStats = attempts
			.Where(a => a.Question is not null)
			.GroupBy(a => a.Question!.Part)
			.Select(g => new PracticePartStatViewModel
			{
				Part = g.Key.ToString(),
				Total = g.Count(),
				Correct = g.Count(x => x.IsCorrect)
			})
			.OrderBy(x => x.Part)
			.ToList();

		var grammarTagStats = attempts
			.Where(a => a.Question is not null && !string.IsNullOrWhiteSpace(a.Question!.GrammarTag))
			.GroupBy(a => a.Question!.GrammarTag)
			.Select(g => new PracticeGrammarTagStatViewModel
			{
				GrammarTag = g.Key,
				Total = g.Count(),
				Incorrect = g.Count(x => !x.IsCorrect)
			})
			.OrderByDescending(x => x.Incorrect)
			.ThenBy(x => x.GrammarTag)
			.ToList();

		var vm = new PracticeSummaryViewModel
		{
			TotalAnswered = totalAnswered,
			TotalCorrect = totalCorrect,
			PartStats = partStats,
			GrammarTagStats = grammarTagStats
		};

		return View(vm);
	}
}
