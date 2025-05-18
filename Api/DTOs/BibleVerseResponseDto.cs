namespace Api.DTOs;

public class BibleVerseResponseDto
{
    public BookDto Book { get; set; }
    public ChapterDto Chapter { get; set; }
    public List<VerseDto> Verses { get; set; }
}
