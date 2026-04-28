using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Application.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "smallint")]
        public short Pages { get; set; }
        
        public DateTime? ReleaseDate { get; set; }
        
        public Guid GenreId { get; set; }

        public Guid AuthorId { get; set; }

        public virtual Author Author { get; set; } = null!;

        public virtual Genre Genre { get; set; } = null!;

        public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
    }
}