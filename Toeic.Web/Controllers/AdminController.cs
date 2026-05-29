using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toeic.Infrastructure.Identity;

namespace Toeic.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
