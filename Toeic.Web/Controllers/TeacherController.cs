using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Domain.Entities;
using Toeic.Infrastructure.Identity;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Models.Teacher;

namespace Toeic.Web.Controllers;

[Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
[Route("Teacher/Classes")]
public class TeacherController : Controller
{
	private readonly AppDbContext _dbContext;

	public TeacherController(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	[HttpGet("")]
	public async Task<IActionResult> Index()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var isAdmin = User.IsInRole(AppRoles.Admin);
		var query = _dbContext.ClassRooms.AsNoTracking().AsQueryable();
		if (!isAdmin)
		{
			query = query.Where(c => c.TeacherId == userId);
		}

		var classes = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
		return View(classes);
	}

	[HttpGet("Create")]
	public IActionResult Create()
	{
		return View(new ClassRoomFormViewModel());
	}

	[HttpPost("Create")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ClassRoomFormViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(teacherId))
		{
			return Challenge();
		}

		var classRoom = new ClassRoom
		{
			Name = model.Name,
			Description = model.Description,
			TeacherId = teacherId,
			CreatedAt = DateTime.UtcNow
		};

		await _dbContext.ClassRooms.AddAsync(classRoom);
		await _dbContext.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Details(int id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return Challenge();
		}

		var classRoom = await _dbContext.ClassRooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		if (classRoom is null)
		{
			return NotFound();
		}

		var isAdmin = User.IsInRole(AppRoles.Admin);
		if (!isAdmin && classRoom.TeacherId != userId)
		{
			return Forbid();
		}

		return View(classRoom);
	}
}
