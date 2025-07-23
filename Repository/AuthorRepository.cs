using BookStore.Data;
using BookStore.Helper;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BookStore.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DataContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IMemoryCache _cache;

        public AuthorRepository(DataContext context, DapperContext dapperContext, IMemoryCache cache)
        {
            _context = context;
            _dapperContext = dapperContext;
            _cache = cache;
        }

        //Tao tác giả
        public async Task<AuthorModel> CreateAuthor(AuthorModel author)
        {

            if (string.IsNullOrWhiteSpace(author.AuthorName))
                throw new ArgumentException("Author name is required.");

            if (await _context.Authors.AnyAsync(x => x.AuthorName == author.AuthorName))
            {
                throw new ArgumentException("Author name already exists.");
            }
                
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            //Xóa cache cũ
            _cache.Remove(CacheKeys.AllAuthors);
            return author;
        }

        //Lay tất cả tác giả
        public async Task<List<AuthorModel>> GetAllAuthors()
        {
            if (_cache.TryGetValue(CacheKeys.AllAuthors, out List<AuthorModel>? authors))
            {
                return authors ?? new List<AuthorModel>();
            }
            
            authors = await _context.Authors.ToListAsync();

            //Lưu cache
            if (authors != null)
            {
                _cache.Set(CacheKeys.AllAuthors, authors, TimeSpan.FromMinutes(10));                
            }
            else 
            {
                return new List<AuthorModel>();
            }
            return authors;
        }

        //Lay tác giả theo id
        public async Task<AuthorModel> GetAuthorById(int authorId)
        {
            if (_cache.TryGetValue(CacheKeys.AuthorById(authorId), out AuthorModel? author))
            {
                return author ?? new AuthorModel();
            }

            author = await _context.Authors.FindAsync(authorId);
            // Kiểm tra null và lưu cache
            if (author != null)
            {
                _cache.Set(CacheKeys.AuthorById(authorId), author, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new AuthorModel();
            }
            return author;
        }

        //Xóa tác giả
        public async Task<bool> DeleteAuthor(int authorId)
        {
            var authorExisting = await _context.Authors.FindAsync(authorId);
            if (authorExisting == null)
            {
                return false;
            }
            _context.Authors.Remove(authorExisting);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.AllPublishers);
            _cache.Remove(CacheKeys.AuthorById(authorId));
            return true;
        }

        //Cập nhật tác giả
        public async Task<AuthorModel> UpdateAuthor(AuthorModel author)
        {
            var existingAuthor = await _context.Authors.FindAsync(author.AuthorId);

            if (existingAuthor == null)
            {
                throw new ArgumentException("Author not found.");
            }

            if (string.IsNullOrWhiteSpace(author.AuthorName))
                throw new ArgumentException("Author name is required.");            

            existingAuthor.AuthorName = author.AuthorName != null ? author.AuthorName : existingAuthor.AuthorName;
            existingAuthor.Biography = author.Biography != null ? author.Biography : existingAuthor.Biography;
            existingAuthor.DateOfDeath = author.DateOfDeath != null ? author.DateOfDeath : existingAuthor.DateOfDeath;
            existingAuthor.DateOfBirth = author.DateOfBirth != null ? author.DateOfBirth : existingAuthor.DateOfBirth;
            existingAuthor.Slug = author.Slug != null ? author.Slug : existingAuthor.Slug;

            _context.Authors.Update(existingAuthor);
            await _context.SaveChangesAsync();

            //Set lại cache
            _cache.Set(CacheKeys.AuthorById(author.AuthorId), existingAuthor, TimeSpan.FromMinutes(10));
            _cache.Remove(CacheKeys.AllAuthors);

            return existingAuthor;
        }

        //Check slug
        public async Task<bool> CheckSlug(string slug)
        {
            return await  _context.Authors.AnyAsync(c => c.Slug == slug);
        }
    }
}
