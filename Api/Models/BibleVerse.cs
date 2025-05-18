namespace Api.Models;

public class BibleVerse
{
    public int Id { get; set; }
    public string Text { get; set; } = "";

    public int BibleId { get; set; }
    public Bible Bible { get; set; } = null!;

    public int VerseId { get; set; }
    public Verse Verse { get; set; } = null!;
}
