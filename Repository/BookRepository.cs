using BookStore.Data;
using BookStore.IRepository;
using BookStore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Dapper;
using System.Data;
using BookStore.Utils;
using System;
using BookStore.Helper;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net;

namespace BookStore.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IMemoryCache _cache;

        public BookRepository(DataContext context, DapperContext dapperContext, IMemoryCache cache)
        {
            _context = context;
            _dapperContext = dapperContext;
            _cache = cache;
        }

        //Lay tat ca book
        public async Task<List<BookModel>> GetAllBooks()
        {
            //Kiểm tra dữ liệu đã có ở cache chưa
            if (_cache.TryGetValue(CacheKeys.AllBooks, out List<BookModel>? books))
            {
                //Trả về mãng rỗng nếu không có dữ liệu
                return books ?? new List<BookModel>();
            }

            books = await _context.Books
                                        .Include(b => b.Author)
                                        .Include(b => b.Category)
                                        .Include(b => b.Publisher)
                                        .ToListAsync();
            //Lưu cache
            if (books != null)
            {
                _cache.Set(CacheKeys.AllBooks, books, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new List<BookModel>();
            }
            return books;
        }

        //Lay book theo id
        public async Task<BookModel> GetBookById(int bookId)
        {
            // Kiểm tra dữ liệu trong cache
            if (_cache.TryGetValue(CacheKeys.BookById(bookId), out BookModel? book))
            {
                return book ?? new BookModel();
            }

            book = await _context.Books
                                        .Include(b => b.Author)
                                        .Include(b => b.Category)
                                        .Include(b => b.Publisher)
                                        .FirstOrDefaultAsync(x => x.BookId == bookId);

            // Kiểm tra null và lưu cache
            if (book != null)
            {
                _cache.Set(CacheKeys.BookById(bookId), book, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new BookModel();
            }
            return book;
        }

        //Lấy book theo slug
        public async Task<BookModel> GetBookBySlug(string slug)
        {
            // Kiểm tra dữ liệu trong cache
            if (_cache.TryGetValue(CacheKeys.BookBySlug(slug), out BookModel? book))
            {
                return book ?? new BookModel();
            }

            book = await _context.Books
                                        .Include(b => b.Author)
                                        .Include(b => b.Category)
                                        .Include(b => b.Publisher)
                                        .FirstOrDefaultAsync(x => x.Slug == slug);

            // Kiểm tra null và lưu cache
            if (book != null)
            {
                _cache.Set(CacheKeys.BookBySlug(slug), book, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new BookModel();
            }
            return book;
        }

        //Tạo book
        public async Task<BookModel> CreateBook(BookModel book)
        {
            if (string.IsNullOrWhiteSpace(book.BookName))
                throw new ArgumentException("Book name is required.");

            if (book.Price <= 0)
                 throw new ArgumentException("Invalid price.");

            if (book.CategoryId == 0 || book.AuthorId == 0 || book.PublisherId == 0)
                 throw new ArgumentException("Invalid category, author, or publisher.");

            if (await _context.Books.AnyAsync(x => x.BookName == book.BookName))
            {
                throw new ArgumentException("Book name already exists.");
            }            

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            //Xóa cache cũ
            _cache.Remove(CacheKeys.AllBooks);            
            _cache.Remove(CacheKeys.Search(string.Empty));

            return book;
        }

        //Sửa book
        public async Task<BookModel> UpdateBook(BookModel book)
        {
            var existingBook = await _context.Books
                                        .Include(b => b.Author)
                                        .Include(b => b.Category)
                                        .Include(b => b.Publisher)
                                        .FirstOrDefaultAsync(b => b.BookId == book.BookId);

                if (existingBook == null)
                {
                    throw new ArgumentException("Book not found.");
                }

                // Kiểm tra dữ liệu nhập vào
                if (string.IsNullOrWhiteSpace(book.BookName))
                    throw new ArgumentException("Book name is required.");

                if (book.Price <= 0)
                    throw new ArgumentException("Invalid price.");

                if (book.CategoryId == 0 || book.AuthorId == 0 || book.PublisherId == 0)
                    throw new ArgumentException("Invalid category, author, or publisher.");

                // Cập nhật dữ liệu sách
                existingBook.BookName = book.BookName != null ? book.BookName : existingBook.BookName;
                existingBook.Description = book.Description ?? existingBook.Description;
                existingBook.Price = book.Price != 0 ? book.Price : existingBook.Price;
                existingBook.AuthorId = book.AuthorId != 0 ? book.AuthorId : existingBook.AuthorId;
                existingBook.CategoryId = book.CategoryId != 0 ? book.CategoryId : existingBook.CategoryId;
                existingBook.PublisherId = book.PublisherId != 0 ? book.PublisherId : existingBook.PublisherId;
                existingBook.CreatedAt = DateTime.UtcNow;
                existingBook.PublishYear = book.PublishYear != 0 ? book.PublishYear : existingBook.PublishYear;
                existingBook.CoverType = book.CoverType ?? existingBook.CoverType;
                existingBook.PageCount = book.PageCount != 0 ? book.PageCount : existingBook.PageCount;
                existingBook.Weight = book.Weight != 0 ? book.Weight : existingBook.Weight;
                existingBook.Size = book.Size ?? existingBook.Size;                
                existingBook.Image = book.Image ?? existingBook.Image;
                existingBook.DiscountPercent = book.DiscountPercent != 0 ? book.DiscountPercent : existingBook.DiscountPercent;
                existingBook.Slug = book.Slug != null ? book.Slug : existingBook.Slug;

                existingBook.Author = await _context.Authors.FindAsync(book.AuthorId);
                existingBook.Category = await _context.Categories.FindAsync(book.CategoryId);
                existingBook.Publisher = await _context.Publishers.FindAsync(book.PublisherId);

            _context.Books.Update(existingBook);
                await _context.SaveChangesAsync();

            //Set lại cache
            _cache.Set(CacheKeys.BookById(book.BookId), existingBook, TimeSpan.FromMinutes(10));
            _cache.Remove(CacheKeys.AllBooks);

            return existingBook;
        }

        //Xoa book
        public async Task<bool> DeleteBook(int bookId)
        {
            var existingBook = await _context.Books.FindAsync(bookId);
            if (existingBook == null)
            {
                return false;
            }
            _context.Books.Remove(existingBook);
            await _context.SaveChangesAsync();

            //Xóa cache
            _cache.Remove(CacheKeys.AllBooks);
            _cache.Remove(CacheKeys.BookById(bookId));
            _cache.Remove(CacheKeys.Search(string.Empty));

            return true;
        }

        //Tim kiếm book
        public async Task<IEnumerable<BookModel>> SearchBook(string keyword)
        {
            if (_cache.TryGetValue(CacheKeys.Search(keyword), out IEnumerable<BookModel>? books))
            {
                return books ?? new List<BookModel>();
            }            

            var parameters = new { keyword };

            using (var connection = _dapperContext.CreateConnection())
            {
                books = await connection.QueryAsync<BookModel>("SearchBook", parameters, commandType: CommandType.StoredProcedure);
            }

            if (books == null)
            {
                return new List<BookModel>();
            }
            
            //Lưu cache
            _cache.Set(CacheKeys.Search(keyword), books, TimeSpan.FromMinutes(10));

            return books;
        }
    
        //Lọc tổng hợp
        public async Task<IEnumerable<BookModel>> FilterBooks(decimal? minPrice, decimal? maxPrice, int? authorId, int? publisherId, int? categoryId)
        {
            if (_cache.TryGetValue(CacheKeys.FilterBook(minPrice, maxPrice, authorId, publisherId, categoryId), out IEnumerable<BookModel>? books))
            {
                return books ?? new List<BookModel>();
            }

            var parameters = new
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                AuthorId = authorId,
                PublisherId = publisherId,
                CategoryId = categoryId
            };

            using (var connection = _dapperContext.CreateConnection())
            {
                books = await connection.QueryAsync<BookModel>("FilterBooks", parameters, commandType: CommandType.StoredProcedure);
            }

            if (books == null)
            {
                return new List<BookModel>();
            }

            //Lưu cache
            _cache.Set(CacheKeys.FilterBook(minPrice, maxPrice, authorId, publisherId, categoryId), books, TimeSpan.FromMinutes(10));

            return books;
        }

        //Check slug
        public async Task<bool> CheckSlug(string slug)
        {
            return await _context.Books.AnyAsync(b => b.Slug == slug);
        }
    }
}
