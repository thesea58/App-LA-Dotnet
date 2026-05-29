using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Grammar;

namespace Toeic.Web.Controllers;

[Authorize]
public class GrammarController : Controller
{
	private readonly AppDbContext _dbContext;

	public GrammarController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IActionResult> Index()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var grouped = await _dbContext.PracticeAttempts
			.Include(x => x.Question)
			.AsNoTracking()
			.Where(x => x.UserId == userId && !x.IsCorrect && x.Question != null)
			.GroupBy(x => x.Question!.GrammarTag)
			.Select(g => new GrammarClinicGroupViewModel
			{
				GrammarTag = g.Key,
				WrongCount = g.Count()
			})
			.OrderByDescending(x => x.WrongCount)
			.ThenBy(x => x.GrammarTag)
			.ToListAsync();

		return View(grouped);
	}

	[HttpGet("/Grammar/Tag")]
	public async Task<IActionResult> Tag(string tag)
	{
		if (string.IsNullOrWhiteSpace(tag))
		{
			return RedirectToAction(nameof(Index));
		}

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var attempts = await _dbContext.PracticeAttempts
			.Include(x => x.Question)
			.AsNoTracking()
			.Where(x => x.UserId == userId && !x.IsCorrect && x.Question != null && x.Question.GrammarTag == tag)
			.OrderByDescending(x => x.AnsweredAt)
			.ToListAsync();

		var vm = new GrammarClinicTagDetailViewModel
		{
			GrammarTag = tag,
			WrongAttempts = attempts
		};

		return View(vm);
	}
}
