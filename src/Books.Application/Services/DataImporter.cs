using System.Globalization;
using Books.Application.Data;
using Books.Application.Dto;
using Books.Application.Mappings;
using Books.Application.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

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
                var author = _context.Authors.Local.FirstOrDefault(a => a.Name == row.Author)
                             ?? _context.Authors.FirstOrDefault(a => a.Name == row.Author)
                             ?? new Author { Id = Guid.NewGuid(), Name = row.Author };
                if (_context.Entry(author).State == EntityState.Detached)
                    _context.Authors.Add(author);

                var genre = _context.Genres.Local.FirstOrDefault(g => g.Name == row.Genre)
                            ?? _context.Genres.FirstOrDefault(g => g.Name == row.Genre)
                            ?? new Genre { Id = Guid.NewGuid(), Name = row.Genre };
                if (_context.Entry(genre).State == EntityState.Detached)
                    _context.Genres.Add(genre);

                var publisher = _context.Publishers.Local.FirstOrDefault(p => p.Name == row.FullPublisherName)
                                ?? _context.Publishers.FirstOrDefault(p => p.Name == row.FullPublisherName)
                                ?? new Publisher { Id = Guid.NewGuid(), Name = row.FullPublisherName };
                if (_context.Entry(publisher).State == EntityState.Detached)
                    _context.Publishers.Add(publisher);

                var book = _context.Books.Local
                               .FirstOrDefault(b => b.Title == row.Title && b.AuthorId == author.Id)
                           ?? _context.Books
                               .Include(b => b.Publishers)
                               .FirstOrDefault(b => b.Title == row.Title && b.AuthorId == author.Id);

                if (book == null)
                {
                    book = new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = row.Title,
                        Pages = (short)row.Pages,
                        ReleaseDate = DateTime.TryParse(row.ReleaseDate, out var date) ? date : null,
                        Author = author,
                        Genre = genre,
                    };
                    _context.Books.Add(book);
                }

                if (!book.Publishers.Any(p => p.Name == publisher.Name))
                    book.Publishers.Add(publisher);
            }

            _context.SaveChanges();
            Console.WriteLine("Import completed!");
        }
    }
}