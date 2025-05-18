using Api.Data;
using Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VersesController : ControllerBase
{
    [HttpGet]
    [Route("{version}/{bookAbbrev}/{chapter}")]
    public async Task<IActionResult> GetVerses(
        string version,
        string bookAbbrev,
        int chapter,
        [FromServices] AppDbContext context
    )
    {
        var verses = await context
            .BibleVerses.Include(bv => bv.Bible)
            .Include(bv => bv.Verse)
            .ThenInclude(v => v.Book)
            .Where(bv => EF.Functions.ILike(bv.Bible.Abbreviation, version))
            .Where(bv => EF.Functions.ILike(bv.Verse.Book.Abbreviation, bookAbbrev))
            .Where(bv => bv.Verse.ChapterNumber == chapter)
            .AsNoTracking()
            .ToListAsync();

        if (!verses.Any())
            return NotFound();

        var versesCount = await context
            .Verses.Where(v => v.Book.Abbreviation == bookAbbrev && v.ChapterNumber == chapter)
            .CountAsync();

        var firstVerse = verses.FirstOrDefault();
        var book = firstVerse.Verse.Book;
        var bible = firstVerse.Bible;

        var response = new BibleVerseResponseDto
        {
            Book = new BookDto
            {
                Abbrev = new AbbrevDto { Pt = book.Abbreviation, En = book.Abbreviation },
                Name = book.Name,
                Version = bible.Abbreviation,
            },
            Chapter = new ChapterDto { Number = chapter, Verses = versesCount },
            Verses = verses
                .Select(bv => new VerseDto { Number = bv.Verse.VerseNumber, Text = bv.Text })
                .OrderBy(v => v.Number)
                .ToList(),
        };

        return Ok(response);
    }

    [HttpGet]
    [Route("{version}/{bookAbbrev}/{chapter}/{number}")]
    public async Task<IActionResult> GetVerse(
        string version,
        string bookAbbrev,
        int chapter,
        int number,
        [FromServices] AppDbContext context
    )
    {
        var verse = await context
            .BibleVerses.Include(bv => bv.Bible)
            .Include(bv => bv.Verse)
            .ThenInclude(v => v.Book)
            .Where(bv => EF.Functions.ILike(bv.Bible.Abbreviation, version))
            .Where(bv => EF.Functions.ILike(bv.Verse.Book.Abbreviation, bookAbbrev))
            .Where(bv => bv.Verse.ChapterNumber == chapter)
            .Where(bv => bv.Verse.VerseNumber == number)
            .FirstOrDefaultAsync();

        if (verse == null)
            return NotFound();

        var response = new BibleVerseResponseDto
        {
            Book = new BookDto
            {
                Abbrev = new AbbrevDto
                {
                    Pt = verse.Verse.Book.Abbreviation,
                    En = verse.Verse.Book.Abbreviation,
                },
                Name = verse.Verse.Book.Name,
                Version = verse.Bible.Abbreviation,
            },
            Chapter = new ChapterDto { Number = verse.Verse.ChapterNumber, Verses = 1 },
            Verses = new List<VerseDto>
            {
                new VerseDto { Number = verse.Verse.VerseNumber, Text = verse.Text },
            },
        };

        return Ok(response);
    }

    [HttpPost]
    [Route("search")]
    public async Task<IActionResult> SearchVerses(
        [FromServices] AppDbContext context,
        [FromBody] SearchVerseRequestDto dto
    )
    {
        if (string.IsNullOrWhiteSpace(dto.Search))
            return BadRequest(new { error = "Search term cannot be empty" });

        if (string.IsNullOrWhiteSpace(dto.Version))
            return BadRequest(new { error = "Version cannot be empty" });

        var verses = await context
            .BibleVerses.Include(bv => bv.Bible)
            .Include(bv => bv.Verse)
            .ThenInclude(v => v.Book)
            .Where(bv => EF.Functions.ILike(bv.Bible.Abbreviation, dto.Version))
            .Where(bv => EF.Functions.ILike(bv.Text, $"%{dto.Search}%"))
            .AsNoTracking()
            .ToListAsync();

        if (!verses.Any())
            return NotFound(new { message = "No verses found matching your search criteria" });

        var groupedVerses = verses
            .GroupBy(v => new { v.Verse.Book.Abbreviation, v.Verse.ChapterNumber })
            .Select(g => new BibleVerseResponseDto
            {
                Book = new BookDto
                {
                    Abbrev = new AbbrevDto
                    {
                        Pt = g.First().Verse.Book.Abbreviation,
                        En = g.First().Verse.Book.Abbreviation,
                    },
                    Name = g.First().Verse.Book.Name,
                    Version = g.First().Bible.Abbreviation,
                },
                Chapter = new ChapterDto { Number = g.Key.ChapterNumber, Verses = g.Count() },
                Verses = g.Select(bv => new VerseDto
                    {
                        Number = bv.Verse.VerseNumber,
                        Text = bv.Text,
                    })
                    .OrderBy(v => v.Number)
                    .ToList(),
            })
            .ToList();

        return Ok(groupedVerses);
    }
}
