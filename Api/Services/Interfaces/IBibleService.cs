using Api.Models;

namespace Api.Services;

public interface IBibleService
{
    public Task<Bible> GetBibleByAbbreviation(string abbreviation);
    public Task<Bible> CreateBible(Bible bible);
}
