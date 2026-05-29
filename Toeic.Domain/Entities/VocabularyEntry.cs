namespace Toeic.Domain.Entities;

public class VocabularyEntry
{
	public int Id { get; set; }
	public string Word { get; set; } = string.Empty;
	public string MeaningVi { get; set; } = string.Empty;
	public string ExampleSentence { get; set; } = string.Empty;
	public string Topic { get; set; } = string.Empty;
	public string PartOfSpeech { get; set; } = string.Empty;
}
