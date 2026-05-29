using Toeic.Domain.Entities;

namespace Toeic.Web.Models.MockTest;

public class MockTestTakeViewModel
{
	public int AttemptId { get; set; }
	public int OrderNo { get; set; }
	public int TotalQuestions { get; set; }
	public Question Question { get; set; } = null!;
}
