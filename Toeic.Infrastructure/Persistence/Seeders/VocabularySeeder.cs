using Toeic.Domain.Entities;

namespace Toeic.Infrastructure.Persistence.Seeders;

public static class VocabularySeeder
{
	public static async Task SeedVocabularyAsync(AppDbContext context)
	{
		if (context.VocabularyEntries.Any())
		{
			return;
		}

		var entries = new List<VocabularyEntry>
		{
			New("agenda", "chương trình họp", "Please review the meeting agenda before 9 a.m.", "Meeting", "Noun"),
			New("minutes", "biên bản họp", "I will send the meeting minutes this afternoon.", "Meeting", "Noun"),
			New("deadline", "hạn chót", "The proposal deadline is next Friday.", "Office", "Noun"),
			New("reschedule", "dời lịch", "We need to reschedule the client call.", "Meeting", "Verb"),
			New("supervisor", "giám sát viên", "Your supervisor will approve this request.", "Office", "Noun"),

			New("itinerary", "lịch trình", "Her travel itinerary includes two cities.", "Travel", "Noun"),
			New("boarding", "lên máy bay", "Boarding starts at Gate 8.", "Travel", "Noun"),
			New("accommodation", "chỗ ở", "The company booked accommodation near the venue.", "Travel", "Noun"),
			New("departure", "khởi hành", "Please confirm your departure time.", "Travel", "Noun"),
			New("commute", "đi làm", "He commutes by train every day.", "Travel", "Verb"),

			New("invoice", "hóa đơn", "Attach the invoice to the payment form.", "Finance", "Noun"),
			New("revenue", "doanh thu", "Quarterly revenue increased by 8 percent.", "Finance", "Noun"),
			New("budget", "ngân sách", "We must stay within the marketing budget.", "Finance", "Noun"),
			New("reimburse", "hoàn chi phí", "Finance will reimburse approved expenses.", "Finance", "Verb"),
			New("outstanding", "còn tồn/chưa thanh toán", "There is one outstanding balance this month.", "Finance", "Adjective"),

			New("applicant", "ứng viên", "Each applicant must submit a CV.", "Recruitment", "Noun"),
			New("interview", "phỏng vấn", "The interview is scheduled for Monday.", "Recruitment", "Noun"),
			New("qualify", "đủ điều kiện", "Two candidates qualify for the next round.", "Recruitment", "Verb"),
			New("onboarding", "hội nhập nhân viên mới", "Onboarding starts on the first day of work.", "Recruitment", "Noun"),
			New("vacancy", "vị trí trống", "The vacancy was posted on our website.", "Recruitment", "Noun"),

			New("discount", "giảm giá", "Members receive a 10 percent discount.", "Shopping", "Noun"),
			New("receipt", "biên lai", "Please keep the receipt for returns.", "Shopping", "Noun"),
			New("refund", "hoàn tiền", "The store processed my refund in two days.", "Shopping", "Noun"),
			New("warranty", "bảo hành", "This printer comes with a one-year warranty.", "Shopping", "Noun"),
			New("purchase", "mua hàng", "All office purchase requests need approval.", "Shopping", "Noun"),

			New("conference", "hội nghị", "She will speak at a sales conference.", "Office", "Noun"),
			New("shipment", "lô hàng", "The shipment arrived earlier than expected.", "Office", "Noun"),
			New("maintenance", "bảo trì", "Routine maintenance will begin tonight.", "Office", "Noun"),
			New("proposal", "đề xuất", "We submitted the final proposal yesterday.", "Office", "Noun"),
			New("negotiate", "đàm phán", "They will negotiate the contract terms tomorrow.", "Meeting", "Verb")
		};

		await context.VocabularyEntries.AddRangeAsync(entries);
		await context.SaveChangesAsync();
	}

	private static VocabularyEntry New(string word, string meaningVi, string exampleSentence, string topic, string partOfSpeech)
	{
		return new VocabularyEntry
		{
			Word = word,
			MeaningVi = meaningVi,
			ExampleSentence = exampleSentence,
			Topic = topic,
			PartOfSpeech = partOfSpeech
		};
	}
}
