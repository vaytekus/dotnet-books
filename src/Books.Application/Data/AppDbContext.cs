using Microsoft.EntityFrameworkCore;
using Books.Application.Models;

namespace Books.Application.Data
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString =
            "Server=localhost;Database=BooksDb;User Id=sa;Password=Password12345;TrustServerCertificate=True;";

        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}