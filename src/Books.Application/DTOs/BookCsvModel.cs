namespace Books.Application.Dto
{
    public class BookCsvModel
    {
        public string Title { get; set; } = string.Empty;
        public int Pages { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string? ReleaseDate { get; set; }

        public string PubPart1 { get; set; } = string.Empty;
        public string PubPart2 { get; set; } = string.Empty;
        public string PubPart3 { get; set; } = string.Empty;
        
        public string FullPublisherName => 
            $"{PubPart1} {PubPart2} {PubPart3}"
                .Replace("  ", " ")
                .Trim();
    }
}