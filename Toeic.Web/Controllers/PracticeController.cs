using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Domain.Enums;
using Toeic.Infrastructure.Persistence;

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
}
