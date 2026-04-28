using Books.Application.Data;
using Books.Application.Filters;
using Books.Application.Models;
using Books.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Books.Application.Tests;

public class FilterTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);

        var author = new Author { Id = Guid.NewGuid(), Name = "Homer" };
        var genre = new Genre { Id = Guid.NewGuid(), Name = "Classic" };
        var publisher = new Publisher { Id = Guid.NewGuid(), Name = "Penguin" };

        context.Books.AddRange(
            new Book { Id = Guid.NewGuid(), Title = "The Odyssey", Pages = 541, ReleaseDate = null, Author = author, Genre = genre, Publisher = publisher },
            new Book { Id = Guid.NewGuid(), Title = "The Iliad", Pages = 683, ReleaseDate = null, Author = author, Genre = genre, Publisher = publisher },
            new Book { Id = Guid.NewGuid(), Title = "Moby Dick", Pages = 720, ReleaseDate = new DateTime(1851, 1, 1), Author = new Author { Id = Guid.NewGuid(), Name = "Melville" }, Genre = new Genre { Id = Guid.NewGuid(), Name = "Adventure" }, Publisher = publisher }
        );
        context.SaveChanges();
        return context;
    }

    [Fact]
    public void FilterByTitle_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { Title = "Odyssey" });

        Assert.Single(result);
        Assert.Equal("The Odyssey", result[0].Title);
    }

    [Fact]
    public void FilterByGenre_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { Genre = "Classic" });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FilterByAuthor_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { Author = "Homer" });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FilterByMoreThanPages_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { MoreThanPages = 600 });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FilterByLessThanPages_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { LessThanPages = 600 });

        Assert.Single(result);
        Assert.Equal("The Odyssey", result[0].Title);
    }

    [Fact]
    public void EmptyFilter_ReturnsAllBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto());

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void FilterByGenreAndMoreThanPages_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { Genre = "Classic", MoreThanPages = 600 });

        Assert.Single(result);
        Assert.Equal("The Iliad", result[0].Title);
    }

    [Fact]
    public void FilterWithNoMatch_ReturnsEmptyList()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { Title = "NonExistentBook" });

        Assert.Empty(result);
    }

    [Fact]
    public void FilterByPublishedAfter_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { PublishedAfter = new DateTime(1800, 1, 1) });

        Assert.Single(result);
        Assert.Equal("Moby Dick", result[0].Title);
    }

    [Fact]
    public void FilterByPublishedBefore_ReturnsMatchingBooks()
    {
        var context = CreateContext();
        var filter = new BookFilterService(context);

        var result = filter.FilterBooks(new BookFilterDto { PublishedBefore = new DateTime(1900, 1, 1) });

        Assert.Single(result);
        Assert.Equal("Moby Dick", result[0].Title);
    }
}