using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Toeic.Domain.Entities;
using Toeic.Infrastructure.Identity;

namespace Toeic.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	// Day 4/6: Identity + TOEIC question bank.
	public DbSet<Question> Questions => Set<Question>();
}
