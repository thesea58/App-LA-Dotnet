using Toeic.Domain.Entities;

namespace Toeic.Web.Models.Vocabulary;

public class FlashcardViewModel
{
	public VocabularyEntry Entry { get; set; } = null!;
	public int Index { get; set; }
	public int Total { get; set; }
	public bool ShowBack { get; set; }
}
