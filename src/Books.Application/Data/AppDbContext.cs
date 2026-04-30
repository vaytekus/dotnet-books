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
                .Property(a => a.Name)                                                                                                                                                                               
                .HasMaxLength(200); 
            
            modelBuilder.Entity<Author>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .Property(g => g.Name)
                .HasMaxLength(50); 
            
            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<Publisher>()
                .Property(p => p.Name)
                .HasMaxLength(200);

            modelBuilder.Entity<Publisher>()
                .HasIndex(p => p.Name)
                .IsUnique();
            
            modelBuilder.Entity<Book>()                                                                                                                                                                              
                .Property(b => b.Title)
                .HasMaxLength(200);
            
            modelBuilder.Entity<Book>()
                .Property(b => b.Pages)
                .HasColumnType("smallint");
            
            modelBuilder.Entity<Book>()
                .HasIndex(b => new { b.Title, b.AuthorId })
                .IsUnique();
            
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Publishers)
                .WithMany(p => p.Books)
                .UsingEntity(j => j.ToTable("BookPublishers"));
        }
    }
}