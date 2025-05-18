using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class BibleService(AppDbContext dbContext) : IBibleService
{
    public async Task<Bible> GetBibleByAbbreviation(string abbreviation)
    {
        try
        {
            var bible = await dbContext
                .Bibles.Where(b => b.Abbreviation == abbreviation)
                .FirstOrDefaultAsync();

            if (bible == null)
                throw new Exception($"Bible with abbreviation '{abbreviation}' not found");

            return bible;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetBibleByAbbreviation: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<Bible> CreateBible(Bible bible)
    {
        try
        {
            var existsBible = await dbContext
                .Bibles.Where(b => b.Abbreviation == bible.Abbreviation)
                .FirstOrDefaultAsync();

            if (existsBible != null)
                throw new Exception(
                    $"Bible with abbreviation '{bible.Abbreviation}' already exists"
                );

            await dbContext.Bibles.AddAsync(bible);
            await dbContext.SaveChangesAsync();

            return bible;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateBible: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
