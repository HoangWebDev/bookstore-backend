using BookStore.Models;

namespace BookStore.IRepository
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryModel>> GetAllCategories();
        public Task<CategoryModel> GetCategoryById(int categoryId);
        public Task<CategoryModel> CreateCategory(CategoryModel category);
        public Task<CategoryModel> UpdateCategory(CategoryModel category);
        public Task<bool> DeleteCategory(int categoryId);
        public Task<bool> CheckSlug(string slug);
    }
}
