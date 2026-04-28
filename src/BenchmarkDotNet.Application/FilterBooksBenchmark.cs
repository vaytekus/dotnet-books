using BenchmarkDotNet.Attributes;
using Books.Application.Data;
using Books.Application.Filters;
using Books.Application.Models;
using Books.Application.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkDotNet.Application;

[MemoryDiagnoser]
[SimpleJob]
public class FilterBooksBenchmark
{
    private SqliteConnection _connection = null!;
    private AppDbContext _context = null!;
    private BookFilterService _filter = null!;

    private readonly BookFilterDto _noFilter = new();
    private readonly BookFilterDto _byTitle = new() { Title = "The" };
    private readonly BookFilterDto _byAuthor = new() { Author = "George" };
    private readonly BookFilterDto _byGenre = new() { Genre = "Fiction" };
    private readonly BookFilterDto _byPages = new() { MoreThanPages = 200, LessThanPages = 500 };
    private readonly BookFilterDto _byDate = new() { PublishedAfter = new DateTime(2000, 1, 1), PublishedBefore = new DateTime(2020, 12, 31) };
    private readonly BookFilterDto _combined = new() { Genre = "Fiction", MoreThanPages = 100, PublishedAfter = new DateTime(1990, 1, 1) };

    [GlobalSetup]
    public void Setup()
    {
        // Keep connection open so the in-memory database persists across EF Core's internal open/close cycles
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        SeedData(_context);

        _filter = new BookFilterService(_context);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Benchmark(Baseline = true, Description = "No filter (all books)")]
    public int NoFilter() => _filter.FilterBooks(_noFilter).Count;

    [Benchmark(Description = "Filter by title")]
    public int ByTitle() => _filter.FilterBooks(_byTitle).Count;

    [Benchmark(Description = "Filter by author")]
    public int ByAuthor() => _filter.FilterBooks(_byAuthor).Count;

    [Benchmark(Description = "Filter by genre")]
    public int ByGenre() => _filter.FilterBooks(_byGenre).Count;

    [Benchmark(Description = "Filter by pages range")]
    public int ByPages() => _filter.FilterBooks(_byPages).Count;

    [Benchmark(Description = "Filter by date range")]
    public int ByDate() => _filter.FilterBooks(_byDate).Count;

    [Benchmark(Description = "Combined filters")]
    public int Combined() => _filter.FilterBooks(_combined).Count;

    private static void SeedData(AppDbContext context)
    {
        var genres = new[]
        {
            new Genre { Id = Guid.NewGuid(), Name = "Fiction" },
            new Genre { Id = Guid.NewGuid(), Name = "Non-Fiction" },
            new Genre { Id = Guid.NewGuid(), Name = "Science" },
            new Genre { Id = Guid.NewGuid(), Name = "History" },
            new Genre { Id = Guid.NewGuid(), Name = "Biography" },
        };

        var authors = new[]
        {
            new Author { Id = Guid.NewGuid(), Name = "George Orwell" },
            new Author { Id = Guid.NewGuid(), Name = "George R.R. Martin" },
            new Author { Id = Guid.NewGuid(), Name = "Stephen King" },
            new Author { Id = Guid.NewGuid(), Name = "J.K. Rowling" },
            new Author { Id = Guid.NewGuid(), Name = "Ernest Hemingway" },
        };

        var publishers = new[]
        {
            new Publisher { Id = Guid.NewGuid(), Name = "Penguin Books" },
            new Publisher { Id = Guid.NewGuid(), Name = "HarperCollins" },
            new Publisher { Id = Guid.NewGuid(), Name = "Random House" },
        };

        context.Genres.AddRange(genres);
        context.Authors.AddRange(authors);
        context.Publishers.AddRange(publishers);
        context.SaveChanges();

        var rng = new Random(42);
        var titles = new[] { "The Great Novel", "A Journey", "The Beginning", "End of Days", "New World", "Lost Times", "The Unknown" };
        var books = new List<Book>(1000);

        for (int i = 0; i < 1000; i++)
        {
            books.Add(new Book
            {
                Id = Guid.NewGuid(),
                Title = $"{titles[i % titles.Length]} {i}",
                Pages = rng.Next(50, 1000),
                ReleaseDate = new DateTime(rng.Next(1950, 2024), rng.Next(1, 13), 1),
                GenreId = genres[i % genres.Length].Id,
                AuthorId = authors[i % authors.Length].Id,
                PublisherId = publishers[i % publishers.Length].Id,
            });
        }

        context.Books.AddRange(books);
        context.SaveChanges();
    }
}
