namespace BookStore.DTO
{
    public class ProductSuggestionDto
    {
        public int BookId { get; set; }
        public string? BookName { get; set; }
        public long Price { get; set; }
        public string? Image { get; set; }

    }
}
