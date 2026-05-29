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

	public DbSet<Question> Questions => Set<Question>();
	public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<AnswerOption>()
			.HasOne(option => option.Question)
			.WithMany(question => question.AnswerOptions)
			.HasForeignKey(option => option.QuestionId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
