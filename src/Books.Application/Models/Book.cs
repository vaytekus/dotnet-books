namespace Books.Application.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = string.Empty;

        public int Pages { get; set; }
        
        public DateTime? ReleaseDate { get; set; }
        
        public Guid GenreId { get; set; }

        public Guid AuthorId { get; set; }

        public Guid PublisherId { get; set; }
        
        public virtual Author Author { get; set; } = null!;
        
        public virtual Genre Genre { get; set; } = null!;
        
        public virtual Publisher Publisher { get; set; } = null!;
    }
}