using Api.Data;
using Api.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BookService(AppDbContext context) : IBookService
{
    public async Task<Book?> GetBookById(int id)
    {
        return await context.Books.FindAsync(id);
    }

    public async Task<List<Book>> GetBooks()
    {
        return await context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookByName(string name)
    {
        return await context.Books.Where(b => b.Name == name).FirstAsync();
    }

    public async Task<Book> GetBookByAbbrev(string abbreviation)
    {
        return await context.Books.Where(b => b.Abbreviation == abbreviation).FirstAsync();
    }
}
