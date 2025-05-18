using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Seeder;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            await ReadFilesAsync();
            Console.WriteLine("Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in main: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static async Task ReadFilesAsync()
    {
        try
        {
            var files = Directory.GetFiles(
                "C:\\Users\\YgorAzambuja\\workspace\\BibleApp\\Seeder\\raw_data"
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Connection string: {connectionString}");
            optionsBuilder.UseNpgsql(connectionString);

            using var dbContext = new AppDbContext(optionsBuilder.Options);

            // Criar ou obter livros uma única vez, já que eles são comuns a todas as versões da Bíblia
            var existingBooks = await dbContext.Books.ToListAsync();

            foreach (var file in files)
            {
                try
                {
                    var jsonBooks = ReadJsonFile(file);
                    var bibleAbbrev = file.Split("\\").Last().Split(".").First();

                    Console.WriteLine($"Processing Bible: {bibleAbbrev}");

                    var bible = new Bible
                    {
                        Name = DetermineBibleName(bibleAbbrev),
                        Abbreviation = bibleAbbrev,
                        Language = DetermineBibleLanguage(bibleAbbrev),
                    };

                    await dbContext.Bibles.AddAsync(bible);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"Bible saved with Id: {bible.Id}");

                    var position = 1;
                    foreach (var jsonBook in jsonBooks)
                    {
                        try
                        {
                            // Verificar se o livro já existe
                            var book = existingBooks.FirstOrDefault(b =>
                                b.Abbreviation == jsonBook.Abbrev
                            );

                            if (book == null)
                            {
                                book = new Book
                                {
                                    Name = jsonBook.Name,
                                    Abbreviation = jsonBook.Abbrev,
                                    ChapterCount = jsonBook.Chapters.Length,
                                    Testament = DetermineTestament(jsonBook.Name),
                                    Position = position++,
                                };

                                await dbContext.Books.AddAsync(book);
                                await dbContext.SaveChangesAsync();
                                existingBooks.Add(book);
                                Console.WriteLine($"Book {book.Name} saved with Id: {book.Id}");
                            }

                            var bibleVerses = new List<BibleVerse>();
                            var chapterIndex = 1;

                            foreach (var chapter in jsonBook.Chapters)
                            {
                                var verseIndex = 1;

                                foreach (var verseText in chapter)
                                {
                                    // Verificar se o versículo já existe
                                    var verse = await dbContext.Verses.FirstOrDefaultAsync(v =>
                                        v.BookId == book.Id
                                        && v.ChapterNumber == chapterIndex
                                        && v.VerseNumber == verseIndex
                                    );

                                    if (verse == null)
                                    {
                                        verse = new Verse
                                        {
                                            BookId = book.Id,
                                            ChapterNumber = chapterIndex,
                                            VerseNumber = verseIndex,
                                        };

                                        await dbContext.Verses.AddAsync(verse);
                                        await dbContext.SaveChangesAsync();
                                    }

                                    // Adicionar o BibleVerse para esta versão da Bíblia
                                    bibleVerses.Add(
                                        new BibleVerse
                                        {
                                            BibleId = bible.Id,
                                            VerseId = verse.Id,
                                            Text = verseText,
                                        }
                                    );

                                    verseIndex++;
                                }
                                chapterIndex++;
                            }

                            // Adicionar os versículos específicos para esta versão da Bíblia
                            await dbContext.BibleVerses.AddRangeAsync(bibleVerses);
                            await dbContext.SaveChangesAsync();
                            Console.WriteLine(
                                $"Added {bibleVerses.Count} verses for book {book.Name} in Bible {bible.Abbreviation}"
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Error processing book {jsonBook.Name}: {ex.Message}"
                            );
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ReadFilesAsync: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static string DetermineBibleName(string abbreviation)
    {
        return abbreviation switch
        {
            "ACF" => "Almeida Corrigida Fiel",
            "ARA" => "Almeida Revista e Atualizada",
            "AS21" => "Almeida Revista e Atualizada 2021",
            "JFAA" => "João Ferreira de Almeida	Atualizada",
            "KFA" => "King James Atualizada",
            "KJF" => "King James Fiel",
            "NAA" => "Nova Almeida Atualizada",
            "NBV" => "Nova Bíblia Viva",
            "NTLH" => "Nova Tradução da Linguagem de Hoje",
            "NVI" => "Nova Versão Internacional",
            "NVT" => "Nova Versão Transformadora",
            "TB" => "Tradução Brasileira",
            _ => abbreviation,
        };
    }

    private static string DetermineBibleLanguage(string abbreviation)
    {
        return abbreviation switch
        {
            "kjv" => "en",
            _ => "pt-BR",
        };
    }

    private static Testament DetermineTestament(string bookName)
    {
        string[] oldTestamentBooks =
        {
            "Gênesis",
            "Genesis",
            "Êxodo",
            "Exodo",
            "Levítico",
            "Levitico",
            "Números",
            "Numeros",
            "Deuteronômio",
            "Deuteronomio",
            "Josué",
            "Josue",
            "Juízes",
            "Juizes",
            "Rute",
            "Samuel",
            "1 Samuel",
            "2 Samuel",
            "Reis",
            "1 Reis",
            "2 Reis",
            "Crônicas",
            "Cronicas",
            "1 Crônicas",
            "1 Cronicas",
            "2 Crônicas",
            "2 Cronicas",
            "Esdras",
            "Neemias",
            "Ester",
            "Jó",
            "Jo",
            "Salmos",
            "Salmo",
            "Provérbios",
            "Proverbios",
            "Eclesiastes",
            "Cânticos",
            "Canticos",
            "Cantares",
            "Isaías",
            "Isaias",
            "Jeremias",
            "Lamentações",
            "Lamentacoes",
            "Ezequiel",
            "Daniel",
            "Oséias",
            "Oseias",
            "Joel",
            "Amós",
            "Amos",
            "Obadias",
            "Jonas",
            "Miquéias",
            "Miqueias",
            "Naum",
            "Habacuque",
            "Sofonias",
            "Ageu",
            "Zacarias",
            "Malaquias",
        };

        return oldTestamentBooks.Contains(bookName) ? Testament.Old : Testament.New;
    }

    private static JsonBook[] ReadJsonFile(string file)
    {
        using var reader = new StreamReader(file);
        using var jsonReader = new JsonTextReader(reader);
        var serializer = new JsonSerializer();
        var data = serializer.Deserialize<JsonBook[]>(jsonReader);
        if (data == null)
            throw new Exception("Failed to deserialize the JSON file.");
        return data;
    }
}

internal class JsonBook
{
    public required string Name { get; set; }
    public required string Abbrev { get; set; }
    public required string[][] Chapters { get; set; }
}
