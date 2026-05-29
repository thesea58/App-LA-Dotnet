namespace Toeic.Domain.Entities;

public class MockTestQuestion
{
	public int Id { get; set; }
	public int MockTestId { get; set; }
	public int QuestionId { get; set; }
	public int OrderNo { get; set; }

	public MockTest? MockTest { get; set; }
	public Question? Question { get; set; }
}
