using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Infrastructure.Persistence;

namespace Toeic.Web.Controllers;

[Authorize]
public class VocabularyController : Controller
{
	private readonly AppDbContext _dbContext;

	public VocabularyController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IActionResult> Index()
	{
		var entries = await _dbContext.VocabularyEntries
			.AsNoTracking()
			.OrderBy(v => v.Topic)
			.ThenBy(v => v.Word)
			.ToListAsync();

		return View(entries);
	}
}
