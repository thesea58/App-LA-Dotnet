using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class PracticeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}