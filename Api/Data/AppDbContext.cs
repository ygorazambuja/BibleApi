using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Bible> Bibles { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Verse> Verses { get; set; }
    public DbSet<BibleVerse> BibleVerses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>().Property(b => b.Testament).HasConversion<int>();
        modelBuilder.Entity<Book>().HasIndex(b => b.Abbreviation).IsUnique();
    }
}
