using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Toeic.Domain.Entities;
using Toeic.Domain.Enums;
using Toeic.Infrastructure.Persistence;
using Toeic.Web.Controllers;

namespace Toeic.Tests;

public class MockTestFlowIntegrationTests
{
	[Fact]
	public async Task Authenticated_User_Can_Start_And_Submit_MockTest()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		using var context = new AppDbContext(options);

		var question = new Question
		{
			Part = ToeicPart.Part5,
			Skill = ToeicSkill.Reading,
			QuestionText = "Choose the correct option.",
			Explanation = "Sample explanation.",
			Difficulty = 1,
			Topic = "Office",
			GrammarTag = "Part of speech",
			AnswerOptions = new List<AnswerOption>
			{
				new() { Label = "A", Text = "option A", IsCorrect = true },
				new() { Label = "B", Text = "option B", IsCorrect = false }
			}
		};

		var mockTest = new MockTest
		{
			Title = "TOEIC Mini Test 01",
			Description = "Test",
			DurationMinutes = 30
		};

		context.Questions.Add(question);
		context.MockTests.Add(mockTest);
		await context.SaveChangesAsync();

		context.MockTestQuestions.Add(new MockTestQuestion
		{
			MockTestId = mockTest.Id,
			QuestionId = question.Id,
			OrderNo = 1
		});
		await context.SaveChangesAsync();

		var controller = new MockTestController(context)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = BuildHttpContext("test-user-id")
			}
		};

		var startResult = await controller.Start(mockTest.Id);
		var startRedirect = Assert.IsType<RedirectToActionResult>(startResult);
		Assert.Equal("Take", startRedirect.ActionName);

		var attemptId = Assert.IsType<int>(startRedirect.RouteValues!["attemptId"]);
		var correctOptionId = question.AnswerOptions.First(x => x.IsCorrect).Id;

		var submitResult = await controller.Answer(attemptId, 1, correctOptionId, "submit");
		var submitRedirect = Assert.IsType<RedirectToActionResult>(submitResult);
		Assert.Equal("Result", submitRedirect.ActionName);

		var attempt = await context.MockTestAttempts.FirstAsync(x => x.Id == attemptId);
		Assert.NotNull(attempt.SubmittedAt);
		Assert.Equal(1, attempt.Score);
	}

	private static HttpContext BuildHttpContext(string userId)
	{
		var context = new DefaultHttpContext();
		var identity = new ClaimsIdentity(
			new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
			authenticationType: "TestAuth");
		context.User = new ClaimsPrincipal(identity);
		return context;
	}
}
