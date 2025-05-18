using Api.Models;

namespace Api.Services.Interfaces;

public interface IBookService
{
    public Task<Book?> GetBookById(int id);
    public Task<List<Book>> GetBooks();
    public Task<Book?> GetBookByName(string name);
    public Task<Book?> GetBookByAbbrev(string abbreviation);
}
