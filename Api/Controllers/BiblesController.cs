using Api.Data;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BiblesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] AppDbContext context)
    {
        var bibles = await context.Bibles.ToListAsync();
        return Ok(bibles);
    }

    [HttpGet]
    [Route("{abbreviation}")]
    public async Task<IActionResult> GetBibleByAbbreviation(
        [FromServices] IBibleService bibleService,
        string abbreviation
    )
    {
        try
        {
            var bible = await bibleService.GetBibleByAbbreviation(abbreviation.ToUpper());
            return Ok(bible);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
