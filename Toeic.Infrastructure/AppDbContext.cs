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
	public DbSet<PracticeAttempt> PracticeAttempts => Set<PracticeAttempt>();
	public DbSet<VocabularyEntry> VocabularyEntries => Set<VocabularyEntry>();
	public DbSet<MockTest> MockTests => Set<MockTest>();
	public DbSet<MockTestQuestion> MockTestQuestions => Set<MockTestQuestion>();
	public DbSet<MockTestAttempt> MockTestAttempts => Set<MockTestAttempt>();
	public DbSet<MockTestAnswer> MockTestAnswers => Set<MockTestAnswer>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<AnswerOption>()
			.HasOne(option => option.Question)
			.WithMany(question => question.AnswerOptions)
			.HasForeignKey(option => option.QuestionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<PracticeAttempt>()
			.HasOne(attempt => attempt.Question)
			.WithMany()
			.HasForeignKey(attempt => attempt.QuestionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<MockTestQuestion>()
			.HasOne(item => item.MockTest)
			.WithMany(test => test.Questions)
			.HasForeignKey(item => item.MockTestId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<MockTestQuestion>()
			.HasOne(item => item.Question)
			.WithMany()
			.HasForeignKey(item => item.QuestionId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<MockTestAttempt>()
			.HasOne(attempt => attempt.MockTest)
			.WithMany()
			.HasForeignKey(attempt => attempt.MockTestId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<MockTestAnswer>()
			.HasOne(answer => answer.MockTestAttempt)
			.WithMany(attempt => attempt.Answers)
			.HasForeignKey(answer => answer.MockTestAttemptId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<MockTestAnswer>()
			.HasOne(answer => answer.Question)
			.WithMany()
			.HasForeignKey(answer => answer.QuestionId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
