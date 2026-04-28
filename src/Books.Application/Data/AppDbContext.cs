using Microsoft.EntityFrameworkCore;
using Books.Application.Models;

namespace Books.Application.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<Publisher>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => new { b.Title, b.AuthorId })
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Publishers)
                .WithMany(p => p.Books)
                .UsingEntity(j => j.ToTable("BookPublishers"));
        }
    }
}