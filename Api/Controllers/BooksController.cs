using Api.Services;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromServices] IBookService service)
    {
        var books = await service.GetBooks();

        return Ok(books);
    }

    [HttpGet]
    [Route("{abbrev}")]
    public async Task<IActionResult> GetBookByAbbrev(
        [FromServices] IBookService service,
        string abbrev
    )
    {
        var book = await service.GetBookByAbbrev(abbrev);
        return Ok(book);
    }
}
