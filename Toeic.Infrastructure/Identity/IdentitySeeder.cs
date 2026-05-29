using Microsoft.AspNetCore.Identity;

namespace Toeic.Infrastructure.Identity;

public class IdentitySeeder
{
	private readonly RoleManager<IdentityRole> _roleManager;

	public IdentitySeeder(RoleManager<IdentityRole> roleManager)
	{
		_roleManager = roleManager;
	}

	public async Task SeedRolesAsync()
	{
		var roles = new[] { AppRoles.Student, AppRoles.Teacher, AppRoles.Admin };

		foreach (var role in roles)
		{
			if (!await _roleManager.RoleExistsAsync(role))
			{
				await _roleManager.CreateAsync(new IdentityRole(role));
			}
		}
	}
}
