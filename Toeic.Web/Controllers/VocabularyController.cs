using Microsoft.AspNetCore.Mvc;

namespace Toeic.Web.Controllers;

public class VocabularyController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}