using Microsoft.EntityFrameworkCore;

namespace Toeic.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	// Chưa cần DbSet entity phức tạp ở Day 3.
}