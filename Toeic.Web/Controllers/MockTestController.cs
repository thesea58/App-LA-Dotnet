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
		var attempt = await _dbContext.MockTestAttempts
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == attemptId);
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

		var vm = new MockTestTakeViewModel
		{
			AttemptId = attemptId,
			OrderNo = orderNo,
			TotalQuestions = totalQuestions,
			Question = mockQuestion.Question
		};

		return View(vm);
	}
}
