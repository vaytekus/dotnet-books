using Microsoft.EntityFrameworkCore;
using Books.Application.Models;

namespace Books.Application.Data
{
    public class AppDbContext : DbContext
    {
        private const string DbFileName = "books.db";
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            var dbPath = Path.Combine(projectRoot, DbFileName);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}