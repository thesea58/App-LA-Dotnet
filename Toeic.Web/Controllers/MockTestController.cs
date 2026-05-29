using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Domain.Entities;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.MockTest;

namespace Toeic.Web.Controllers;

[Authorize]
public class MockTestController : Controller
{
	private readonly AppDbContext _dbContext;

	public MockTestController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IActionResult> Index()
	{
		var tests = await _dbContext.MockTests
			.AsNoTracking()
			.OrderBy(x => x.Id)
			.ToListAsync();

		return View(tests);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Start(int mockTestId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var exists = await _dbContext.MockTests.AnyAsync(x => x.Id == mockTestId);
		if (!exists)
		{
			return NotFound();
		}

		var attempt = new MockTestAttempt
		{
			UserId = userId,
			MockTestId = mockTestId,
			StartedAt = DateTime.UtcNow
		};

		await _dbContext.MockTestAttempts.AddAsync(attempt);
		await _dbContext.SaveChangesAsync();

		return RedirectToAction(nameof(Take), new { attemptId = attempt.Id, orderNo = 1 });
	}

	[HttpGet("/MockTest/Take/{attemptId:int}")]
	public async Task<IActionResult> Take(int attemptId, int orderNo = 1)
	{
		var attempt = await GetUserAttemptAsync(attemptId);
		if (attempt is null)
		{
			return NotFound();
		}

		var totalQuestions = await _dbContext.MockTestQuestions
			.CountAsync(x => x.MockTestId == attempt.MockTestId);

		if (orderNo < 1 || orderNo > totalQuestions)
		{
			return RedirectToAction(nameof(Index));
		}

		var mockQuestion = await _dbContext.MockTestQuestions
			.Include(x => x.Question)
			.ThenInclude(q => q!.AnswerOptions)
			.AsNoTracking()
			.Where(x => x.MockTestId == attempt.MockTestId && x.OrderNo == orderNo)
			.FirstOrDefaultAsync();

		if (mockQuestion?.Question is null)
		{
			return NotFound();
		}

		var selectedId = await _dbContext.MockTestAnswers
			.Where(a => a.MockTestAttemptId == attemptId && a.QuestionId == mockQuestion.QuestionId)
			.Select(a => (int?)a.SelectedAnswerOptionId)
			.FirstOrDefaultAsync();

		var vm = new MockTestTakeViewModel
		{
			AttemptId = attemptId,
			OrderNo = orderNo,
			TotalQuestions = totalQuestions,
			Question = mockQuestion.Question,
			SelectedAnswerOptionId = selectedId
		};

		return View(vm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Answer(int attemptId, int orderNo, int selectedAnswerOptionId, string command)
	{
		var attempt = await GetUserAttemptAsync(attemptId);
		if (attempt is null)
		{
			return NotFound();
		}

		var mockQuestion = await _dbContext.MockTestQuestions
			.Include(x => x.Question)
			.ThenInclude(q => q!.AnswerOptions)
			.FirstOrDefaultAsync(x => x.MockTestId == attempt.MockTestId && x.OrderNo == orderNo);

		if (mockQuestion?.Question is null)
		{
			return NotFound();
		}

		var correctOptionId = mockQuestion.Question.AnswerOptions.FirstOrDefault(o => o.IsCorrect)?.Id;
		if (correctOptionId is null)
		{
			return BadRequest("Question does not have a correct option.");
		}

		var existing = await _dbContext.MockTestAnswers.FirstOrDefaultAsync(a =>
			a.MockTestAttemptId == attemptId && a.QuestionId == mockQuestion.QuestionId);

		if (existing is null)
		{
			existing = new MockTestAnswer
			{
				MockTestAttemptId = attemptId,
				QuestionId = mockQuestion.QuestionId
			};
			await _dbContext.MockTestAnswers.AddAsync(existing);
		}

		existing.SelectedAnswerOptionId = selectedAnswerOptionId;
		existing.IsCorrect = selectedAnswerOptionId == correctOptionId.Value;

		await _dbContext.SaveChangesAsync();

		var totalQuestions = await _dbContext.MockTestQuestions.CountAsync(x => x.MockTestId == attempt.MockTestId);
		var isSubmit = string.Equals(command, "submit", StringComparison.OrdinalIgnoreCase) || orderNo >= totalQuestions;

		if (isSubmit)
		{
			attempt.SubmittedAt = DateTime.UtcNow;
			attempt.Score = await _dbContext.MockTestAnswers
				.CountAsync(a => a.MockTestAttemptId == attemptId && a.IsCorrect);

			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(Result), new { attemptId });
		}

		return RedirectToAction(nameof(Take), new { attemptId, orderNo = orderNo + 1 });
	}

	[HttpGet("/MockTest/Result/{attemptId:int}")]
	public async Task<IActionResult> Result(int attemptId)
	{
		var attempt = await GetUserAttemptAsync(attemptId);
		if (attempt is null)
		{
			return NotFound();
		}

		var test = await _dbContext.MockTests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == attempt.MockTestId);
		if (test is null)
		{
			return NotFound();
		}

		var questions = await _dbContext.MockTestQuestions
			.Include(x => x.Question)
			.ThenInclude(q => q!.AnswerOptions)
			.AsNoTracking()
			.Where(x => x.MockTestId == attempt.MockTestId)
			.OrderBy(x => x.OrderNo)
			.ToListAsync();

		var answers = await _dbContext.MockTestAnswers
			.AsNoTracking()
			.Where(x => x.MockTestAttemptId == attemptId)
			.ToListAsync();

		var wrongItems = new List<MockTestWrongItemViewModel>();
		foreach (var item in questions)
		{
			if (item.Question is null)
			{
				continue;
			}

			var answer = answers.FirstOrDefault(a => a.QuestionId == item.QuestionId);
			var correct = item.Question.AnswerOptions.FirstOrDefault(o => o.IsCorrect);
			if (correct is null)
			{
				continue;
			}

			if (answer is null || !answer.IsCorrect)
			{
				var selected = item.Question.AnswerOptions.FirstOrDefault(o => o.Id == answer?.SelectedAnswerOptionId);
				wrongItems.Add(new MockTestWrongItemViewModel
				{
					QuestionId = item.Question.Id,
					QuestionText = item.Question.QuestionText,
					Explanation = item.Question.Explanation,
					SelectedAnswer = selected is null ? "(No answer)" : $"{selected.Label}. {selected.Text}",
					CorrectAnswer = $"{correct.Label}. {correct.Text}"
				});
			}
		}

		var correctCount = answers.Count(a => a.IsCorrect);
		var vm = new MockTestResultViewModel
		{
			AttemptId = attemptId,
			TestTitle = test.Title,
			CorrectCount = correctCount,
			TotalQuestions = questions.Count,
			WrongItems = wrongItems
		};

		return View(vm);
	}

	private async Task<MockTestAttempt?> GetUserAttemptAsync(int attemptId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return null;
		}

		return await _dbContext.MockTestAttempts.FirstOrDefaultAsync(x => x.Id == attemptId && x.UserId == userId);
	}
}
