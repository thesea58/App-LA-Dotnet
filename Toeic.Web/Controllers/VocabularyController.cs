using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Vocabulary;

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

	[HttpGet("/Vocabulary/Flashcards")]
	public async Task<IActionResult> Flashcards(int index = 0, bool showBack = false)
	{
		var entries = await _dbContext.VocabularyEntries
			.AsNoTracking()
			.OrderBy(v => v.Topic)
			.ThenBy(v => v.Word)
			.ToListAsync();

		if (!entries.Any())
		{
			return View("NoFlashcards");
		}

		if (index < 0)
		{
			index = 0;
		}
		if (index >= entries.Count)
		{
			index = 0;
		}

		var vm = new FlashcardViewModel
		{
			Entry = entries[index],
			Index = index,
			Total = entries.Count,
			ShowBack = showBack
		};

		return View(vm);
	}
}
