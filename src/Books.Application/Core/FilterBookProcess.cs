using System.Text.Json;
using Books.Application.Data;
using Books.Application.Filters;
using Books.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Books.Application.Core
{
    public class FilterBookProcess(AppDbContext dbContext)
    {
        public void Process(string inputCsvPath, string inputFilterFile, string outputPath)
        {
            dbContext.Database.Migrate();
            var importer = new DataImporter(dbContext);
            importer.ImportFromCsv(inputCsvPath);
            
            string jsonString = File.ReadAllText(inputFilterFile);
            var filterFile = JsonSerializer.Deserialize<BookFilterDto>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new BookFilterDto();

            var filter = new BookFilterService(dbContext);
            var result = filter.FilterBooks(filterFile);
            
            using var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);
            Console.WriteLine($"Books result {result.Count}");
            foreach (var book in result)
            {
                Console.WriteLine(book.Title);
                var publishers = string.Join(", ", book.Publishers.Select(p => p.Name));
                writer.WriteLine($"{book.Title} | {book.Author.Name} | {book.Genre.Name} | {publishers} | {book.Pages} | {book.ReleaseDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
            }
        }
        
    }
}