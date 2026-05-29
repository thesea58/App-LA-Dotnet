using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class MockTestController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}