using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class GrammarController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}