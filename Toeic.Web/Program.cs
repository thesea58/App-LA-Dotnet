using Microsoft.EntityFrameworkCore;
using Toeic.Infrastructure;
using Toeic.Infrastructure.Identity;
using Toeic.Infrastructure.Persistence;
using Toeic.Infrastructure.Persistence.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.Migrate();

	var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
	await seeder.SeedRolesAsync();
	await QuestionSeeder.SeedPart5QuestionsAsync(dbContext);
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages();

app.Run();
