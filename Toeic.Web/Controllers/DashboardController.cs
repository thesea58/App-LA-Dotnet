using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class DashboardController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}