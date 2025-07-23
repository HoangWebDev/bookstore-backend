namespace BookStore.Request
{
    public class FilterBooksRequest
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
        public int? CategoryId { get; set; }
    }
}
