namespace Api.Models;

public class Bible
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
    public string Language { get; set; } = "";

    public ICollection<BibleVerse> Verses { get; set; } = new List<BibleVerse>();
}
