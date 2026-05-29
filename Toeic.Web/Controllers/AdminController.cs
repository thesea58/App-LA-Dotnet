using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Infrastructure.Identity;
using Toeic.Infrastructure.Persistence;

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
}
