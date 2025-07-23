using BookStore.Models;
using System.Collections.Generic;

namespace BookStore.IRepository
{
    public interface IBookRepository
    {
        public Task<List<BookModel>> GetAllBooks();
        public Task<BookModel> GetBookById(int bookId);
        public Task<BookModel> GetBookBySlug(string slug);
        public Task<BookModel> CreateBook(BookModel book);
        public Task<BookModel> UpdateBook(BookModel book);
        public Task<bool> DeleteBook(int bookId);
        public Task<IEnumerable<BookModel>> SearchBook(string title);
        public Task<IEnumerable<BookModel>> FilterBooks(decimal? minPrice, decimal? maxPrice, int? authorId, int? publisherId, int? categoryId);
        Task<bool> CheckSlug(string slug);
    }
}
