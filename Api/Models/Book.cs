namespace Api.Models;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public required string Abbreviation { get; set; }
    public int ChapterCount { get; set; }
    public Testament Testament { get; set; }
    public int Position { get; set; } // Posição do livro na Bíblia

    public ICollection<Verse> Verses { get; set; } = new List<Verse>();
}
