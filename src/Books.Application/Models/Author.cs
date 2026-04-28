using System.ComponentModel.DataAnnotations;

namespace Books.Application.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}