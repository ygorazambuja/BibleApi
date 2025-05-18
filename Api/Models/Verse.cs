namespace Api.Models;

public class Verse
{
    public int Id { get; set; }
    public int ChapterNumber { get; set; }
    public int VerseNumber { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    // Não há texto, pois o texto varia conforme a versão da Bíblia
    // O texto está em BibleVerse
}
