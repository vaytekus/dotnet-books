using Books.Application.Data;
using Books.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Books.Application.Tests;

public class DataImporterTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private string CreateCsvFile(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void ImportFromCsv_ImportsBooks()
    {
        var context = CreateContext();
        var importer = new DataImporter(context);
        var csv = CreateCsvFile("Title,Pages,Genre,ReleaseDate,Author,Publisher\nThe Odyssey,541,Classic,8th century BC,Homer,Penguin");

        importer.ImportFromCsv(csv);

        Assert.Equal(1, context.Books.Count());
        Assert.Equal("The Odyssey", context.Books.First().Title);
    }

    [Fact]
    public void ImportFromCsv_DoesNotCreateDuplicates()
    {
        var context = CreateContext();
        var importer = new DataImporter(context);
        var csv = CreateCsvFile("Title,Pages,Genre,ReleaseDate,Author,Publisher\nThe Odyssey,541,Classic,8th century BC,Homer,Penguin");

        importer.ImportFromCsv(csv);
        importer.ImportFromCsv(csv);

        Assert.Equal(1, context.Books.Count());
    }

    [Fact]
    public void ImportFromCsv_InvalidDate_SetsNull()
    {
        var context = CreateContext();
        var importer = new DataImporter(context);
        var csv = CreateCsvFile("Title,Pages,Genre,ReleaseDate,Author,Publisher\nThe Odyssey,541,Classic,8th century BC,Homer,Penguin");

        importer.ImportFromCsv(csv);

        Assert.Null(context.Books.First().ReleaseDate);
    }

    [Fact]
    public void ImportFromCsv_ValidDate_ParsesCorrectly()
    {
        var context = CreateContext();
        var importer = new DataImporter(context);
        var csv = CreateCsvFile("Title,Pages,Genre,ReleaseDate,Author,Publisher\nMoby Dick,720,Adventure,1851-01-01,Melville,Penguin");

        importer.ImportFromCsv(csv);

        Assert.Equal(new DateTime(1851, 1, 1), context.Books.First().ReleaseDate);
    }

    [Fact]
    public void ImportFromCsv_MultiPartPublisher_CombinesCorrectly()
    {
        var context = CreateContext();
        var importer = new DataImporter(context);
        var csv = CreateCsvFile("Title,Pages,Genre,ReleaseDate,Author,Publisher\nSome Book,300,Fiction,2000-01-01,Some Author,Little, Brown and Company");

        importer.ImportFromCsv(csv);

        Assert.Equal("Little Brown and Company", context.Publishers.First().Name);
    }
}