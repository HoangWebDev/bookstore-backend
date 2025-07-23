namespace BookStore.Utils
{
    public static class CacheKeys
    {
        //Books
        public static string AllBooks => "all_books";
        public static string BookById(int id) => $"book_{id}";
        public static string BookBySlug(string slug) => $"book_{slug}";
        public static string Search(string title) => $"search_{title}";        
        public static string FilterBook(
            decimal? minPrice, 
            decimal? maxPrice, 
            int? authorId, 
            int? publisherId, 
            int? categoryId) => $"filter_book_{minPrice}_{maxPrice}_{authorId}_{publisherId}_{categoryId}";

        //Category
        public static string AllCategories => "all_categories";
        public static string CategoryById(int id) => $"category_{id}";

        //Author
        public static string AllAuthors => "all_authors";
        public static string AuthorById(int id) => $"author_{id}";

        //Publisher
        public static string AllPublishers => "all_publishers";
        public static string PublisherById(int id) => $"publisher_{id}";

        //User
        public static string UserById(int id) => $"user_{id}";
        public static string AllUsers => "all_users";

        //Chat
        public static string ChatCache(int senderId, int receiverId) => $"{Math.Min(senderId, receiverId)}_{Math.Max(senderId, receiverId)}";
    }
}
