using System.Globalization;
using Books.Application.Data;
using Books.Application.Dto;
using Books.Application.Mappings;
using Books.Application.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace Books.Application.Services
{
    public class DataImporter
    {
        private readonly AppDbContext _context;

        public DataImporter(AppDbContext context) => _context = context;

        public void ImportFromCsv(string csvFilePath)
        {
            using var reader = new StreamReader(csvFilePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
            };
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<BookMap>();

            var records = csv.GetRecords<BookCsvModel>().ToList();

            foreach (var row in records)
            {
                var author = _context.Authors.FirstOrDefault(a => a.Name == row.Author)
                             ?? new Author { Id = Guid.NewGuid(), Name = row.Author };
                if (_context.Entry(author).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                    _context.Authors.Add(author);

                var genre = _context.Genres.FirstOrDefault(g => g.Name == row.Genre)
                            ?? new Genre { Id = Guid.NewGuid(), Name = row.Genre };
                if (_context.Entry(genre).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                    _context.Genres.Add(genre);

                var publisher = _context.Publishers.FirstOrDefault(p => p.Name == row.FullPublisherName)
                                ?? new Publisher { Id = Guid.NewGuid(), Name = row.FullPublisherName };
                if (_context.Entry(publisher).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                    _context.Publishers.Add(publisher);

                var exists = _context.Books.Any(b => b.Title == row.Title && b.Author == author);
                if (!exists)
                {
                    var book = new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = row.Title,
                        Pages = row.Pages,
                        ReleaseDate = DateTime.TryParse(row.ReleaseDate, out var date) ? date : null,
                        Author = author,
                        Genre = genre,
                        Publisher = publisher
                    };
                    _context.Books.Add(book);
                }
            }

            _context.SaveChanges();
            Console.WriteLine("Import completed!");
        }
    }
}