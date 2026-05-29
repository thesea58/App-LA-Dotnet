using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
	private readonly UserManager<ApplicationUser> _userManager;

	public TeacherController(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		_dbContext = dbContext;
		_userManager = userManager;
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
		var classRoom = await GetAuthorizedClassRoomAsync(id);
		if (classRoom is null)
		{
			return NotFound();
		}

		var enrollments = await _dbContext.ClassEnrollments
			.AsNoTracking()
			.Where(x => x.ClassRoomId == id)
			.OrderByDescending(x => x.EnrolledAt)
			.ToListAsync();

		var studentIds = enrollments.Select(x => x.StudentId).Distinct().ToList();
		var users = await _userManager.Users
			.Where(u => studentIds.Contains(u.Id))
			.ToDictionaryAsync(x => x.Id, x => x.Email ?? string.Empty);

		var vm = new ClassDetailsViewModel
		{
			ClassRoom = classRoom,
			Students = enrollments.Select(e => new ClassStudentViewModel
			{
				StudentId = e.StudentId,
				Email = users.TryGetValue(e.StudentId, out var email) ? email : "(unknown)",
				EnrolledAt = e.EnrolledAt
			}).ToList()
		};

		return View(vm);
	}

	[HttpPost("{id:int}/Enroll")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> EnrollStudent(int id, string studentEmail)
	{
		var classRoom = await GetAuthorizedClassRoomAsync(id);
		if (classRoom is null)
		{
			return NotFound();
		}

		if (string.IsNullOrWhiteSpace(studentEmail))
		{
			TempData["TeacherError"] = "Student email is required.";
			return RedirectToAction(nameof(Details), new { id });
		}

		var user = await _userManager.FindByEmailAsync(studentEmail.Trim());
		if (user is null)
		{
			TempData["TeacherError"] = "Student email not found.";
			return RedirectToAction(nameof(Details), new { id });
		}

		var exists = await _dbContext.ClassEnrollments.AnyAsync(x => x.ClassRoomId == id && x.StudentId == user.Id);
		if (exists)
		{
			TempData["TeacherError"] = "Student is already enrolled in this class.";
			return RedirectToAction(nameof(Details), new { id });
		}

		await _dbContext.ClassEnrollments.AddAsync(new ClassEnrollment
		{
			ClassRoomId = id,
			StudentId = user.Id,
			EnrolledAt = DateTime.UtcNow
		});
		await _dbContext.SaveChangesAsync();

		TempData["TeacherSuccess"] = "Student enrolled successfully.";
		return RedirectToAction(nameof(Details), new { id });
	}

	private async Task<ClassRoom?> GetAuthorizedClassRoomAsync(int id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId))
		{
			return null;
		}

		var classRoom = await _dbContext.ClassRooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		if (classRoom is null)
		{
			return null;
		}

		var isAdmin = User.IsInRole(AppRoles.Admin);
		if (!isAdmin && classRoom.TeacherId != userId)
		{
			return null;
		}

		return classRoom;
	}
}
