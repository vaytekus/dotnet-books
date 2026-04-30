namespace Books.Application.Models
{
    public class Publisher
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}