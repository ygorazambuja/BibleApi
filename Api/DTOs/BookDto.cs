namespace Api.DTOs;

public class BookDto
{
    public AbbrevDto Abbrev { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Group { get; set; }
    public string Version { get; set; }
}
