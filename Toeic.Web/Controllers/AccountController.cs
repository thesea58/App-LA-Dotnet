using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Toeic.Infrastructure.Identity;
using Toeic.Web.Models.Account;

namespace Toeic.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public AccountController(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[HttpGet]
	public IActionResult Register(string? returnUrl = null)
	{
		ViewData["ReturnUrl"] = returnUrl ?? Url.Action("Index", "Dashboard");
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
	{
		returnUrl ??= Url.Action("Index", "Dashboard") ?? "/";

		if (!ModelState.IsValid)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
		}

		var user = new ApplicationUser
		{
			UserName = model.Email,
			Email = model.Email
		};

		var createResult = await _userManager.CreateAsync(user, model.Password);
		if (createResult.Succeeded)
		{
			await _userManager.AddToRoleAsync(user, AppRoles.Student);
			await _signInManager.SignInAsync(user, isPersistent: false);
			return LocalRedirect(returnUrl);
		}

		foreach (var error in createResult.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		ViewData["ReturnUrl"] = returnUrl;
		return View(model);
	}

	[HttpGet]
	public IActionResult Login(string? returnUrl = null)
	{
		ViewData["ReturnUrl"] = returnUrl ?? Url.Action("Index", "Dashboard");
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
	{
		returnUrl ??= Url.Action("Index", "Dashboard") ?? "/";

		if (!ModelState.IsValid)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
		}

		var result = await _signInManager.PasswordSignInAsync(
			model.Email,
			model.Password,
			model.RememberMe,
			lockoutOnFailure: false);

		if (result.Succeeded)
		{
			return LocalRedirect(returnUrl);
		}

		ModelState.AddModelError(string.Empty, "Invalid login attempt.");
		ViewData["ReturnUrl"] = returnUrl;
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction("Index", "Home");
	}

	public IActionResult AccessDenied()
	{
		return View();
	}
}
