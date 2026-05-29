using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class AdminController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}