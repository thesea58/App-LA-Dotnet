using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Dashboard;

namespace Toeic.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
	private readonly AppDbContext _dbContext;

	public DashboardController(AppDbContext dbContext)
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

		var attempts = await _dbContext.PracticeAttempts
			.Include(a => a.Question)
			.AsNoTracking()
			.Where(a => a.UserId == userId)
			.ToListAsync();

		var totalPractice = attempts.Count;
		var overallAccuracy = totalPractice == 0 ? 0 : Math.Round((double)attempts.Count(a => a.IsCorrect) / totalPractice * 100, 2);

		var partStats = attempts
			.Where(a => a.Question is not null)
			.GroupBy(a => a.Question!.Part)
			.Select(g => new DashboardPartStatViewModel
			{
				Part = g.Key.ToString(),
				Accuracy = g.Any() ? Math.Round((double)g.Count(x => x.IsCorrect) / g.Count() * 100, 2) : 0
			})
			.OrderBy(x => x.Part)
			.ToList();

		var submittedMocks = await _dbContext.MockTestAttempts
			.AsNoTracking()
			.Where(x => x.UserId == userId && x.SubmittedAt != null)
			.OrderByDescending(x => x.SubmittedAt)
			.ToListAsync();

		var weakestGrammarTag = attempts
			.Where(a => a.Question is not null && !a.IsCorrect)
			.GroupBy(a => a.Question!.GrammarTag)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key)
			.FirstOrDefault() ?? "N/A";

		var vm = new DashboardViewModel
		{
			TotalPracticeQuestions = totalPractice,
			OverallAccuracy = overallAccuracy,
			PartStats = partStats,
			MockTestsCompleted = submittedMocks.Count,
			LatestMockTestScore = submittedMocks.FirstOrDefault()?.Score,
			WeakestGrammarTag = weakestGrammarTag
		};

		var recommendations = new List<string>();
		var weakestPart = partStats.OrderBy(x => x.Accuracy).FirstOrDefault();
		if (weakestPart is not null)
		{
			recommendations.Add($"Practice more questions in {weakestPart.Part}.");
		}

		if (!string.Equals(weakestGrammarTag, "N/A", StringComparison.OrdinalIgnoreCase))
		{
			recommendations.Add($"Open Grammar Clinic for '{weakestGrammarTag}'.");
		}

		if (submittedMocks.Count == 0)
		{
			recommendations.Add("Start TOEIC Mini Test 01 to benchmark your level.");
		}

		vm.Recommendations = recommendations;

		vm.StudentAssignments = await _dbContext.Assignments
			.Include(a => a.ClassRoom)
			.Include(a => a.MockTest)
			.AsNoTracking()
			.Where(a => _dbContext.ClassEnrollments.Any(e => e.ClassRoomId == a.ClassRoomId && e.StudentId == userId))
			.OrderBy(a => a.DueDate)
			.Select(a => new StudentAssignmentViewModel
			{
				ClassName = a.ClassRoom != null ? a.ClassRoom.Name : "(unknown)",
				Title = a.Title,
				MockTestTitle = a.MockTest != null ? a.MockTest.Title : "(unknown)",
				DueDate = a.DueDate
			})
			.ToListAsync();

		return View(vm);
	}
}
