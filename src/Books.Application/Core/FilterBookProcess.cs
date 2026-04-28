using System.Text.Json;
using Books.Application.Data;
using Books.Application.Filters;
using Books.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Books.Application.Core
{
    public class FilterBookProcess(string inputCsvPath, string inputFilterFile, string outputPath, DbContextOptions<AppDbContext> dbOptions)
    {
        public void Process()
        {
            var context = new AppDbContext(dbOptions);
            context.Database.Migrate();
            var importer = new DataImporter(context);
            importer.ImportFromCsv(inputCsvPath);
            
            string jsonString = File.ReadAllText(inputFilterFile);
            var filterFile = JsonSerializer.Deserialize<BookFilterDto>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new BookFilterDto();

            var filter = new BookFilterService(context);
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