namespace Api.DTOs;

public record SearchVerseRequestDto
{
    public string Version { get; set; }
    public string Search { get; set; }
}
