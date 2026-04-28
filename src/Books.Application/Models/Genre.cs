using System.ComponentModel.DataAnnotations;

namespace Books.Application.Models
{
    public class Genre
    {
        public Guid Id { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}