using Books.Application.Dto;
using CsvHelper.Configuration;

namespace Books.Application.Mappings
{
    public class BookMap : ClassMap<BookCsvModel>
    {
        public BookMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.Pages).Name("Pages");
            Map(m => m.Genre).Name("Genre");
            Map(m => m.ReleaseDate).Name("ReleaseDate");
            Map(m => m.Author).Name("Author");
            
            Map(m => m.PubPart1).Name("Publisher");
            Map(m => m.PubPart2).Index(6).Optional();
            Map(m => m.PubPart3).Index(7).Optional();
        }
    }
}