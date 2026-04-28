using Books.Application.Data;
using Books.Application.Filters;
using Books.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Books.Application.Services
{
    public class BookFilterService
    {
        private readonly AppDbContext _context;

        public BookFilterService(AppDbContext context) => _context = context;

        public List<Book> FilterBooks(BookFilterDto filter)
        {
            var query = _context.Books
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.Publisher)
                .AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(b => b.Title.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.Author))
                query = query.Where(b => b.Author.Name.Contains(filter.Author));

            if (!string.IsNullOrWhiteSpace(filter.Genre))
                query = query.Where(b => b.Genre.Name.Contains(filter.Genre));

            if (!string.IsNullOrWhiteSpace(filter.Publisher))
                query = query.Where(b => b.Publisher.Name.Contains(filter.Publisher));

            if (filter.MoreThanPages.HasValue)
                query = query.Where(b => b.Pages >= filter.MoreThanPages.Value);

            if (filter.LessThanPages.HasValue)
                query = query.Where(b => b.Pages <= filter.LessThanPages.Value);

            if (filter.PublishedAfter.HasValue)
                query = query.Where(b => b.ReleaseDate >= filter.PublishedAfter.Value);

            if (filter.PublishedBefore.HasValue)
                query = query.Where(b => b.ReleaseDate <= filter.PublishedBefore.Value);

            return query.ToList();
        }
    }
}