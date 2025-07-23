using BookStore.Models;

namespace BookStore.IRepository
{
    public interface IAuthorRepository
    {
        public Task<List<AuthorModel>> GetAllAuthors();
        public Task<AuthorModel> GetAuthorById(int authorId);
        public Task<AuthorModel> CreateAuthor(AuthorModel authorModel);
        public Task<AuthorModel> UpdateAuthor(AuthorModel authorModel);
        public Task<bool> DeleteAuthor(int authorId);
        public Task<bool> CheckSlug(string slug);
    }
}
