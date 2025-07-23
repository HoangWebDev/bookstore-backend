using BookStore.Data;
using BookStore.IRepository;
using BookStore.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Dapper;
using BookStore.Utils;
using BookStore.Helper;

namespace BookStore.Repository
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly DataContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IMemoryCache _cache;

        public CategoryRepository(DataContext context, DapperContext dapperContext, IMemoryCache cache)
        {
            _context = context;
            _dapperContext = dapperContext;
            _cache = cache;
        }

        //Tao danh mucch
        public async Task<CategoryModel> CreateCategory(CategoryModel category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required.");

            if (await _context.Categories.AnyAsync(x => x.CategoryName == category.CategoryName))
            {
                throw new ArgumentException("Category name already exists.");
            }           

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            //Xóa cache cũ
            _cache.Remove(CacheKeys.AllCategories);
            return category;
        }

        //Lay tat ca danh mục
        public async Task<List<CategoryModel>> GetAllCategories()
        {
            if (_cache.TryGetValue(CacheKeys.AllCategories, out List<CategoryModel>? categories))
            {
                return categories ?? new List<CategoryModel>();
            }

            categories = await _context.Categories.ToListAsync();
            //Lưu cache
            if (categories != null)
            {
                _cache.Set(CacheKeys.AllCategories, categories, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new List<CategoryModel>();
            }
            return categories;
        }

        //Lay danh mục theo id
        public async Task<CategoryModel> GetCategoryById(int categoryId)
        {
            if (_cache.TryGetValue(CacheKeys.CategoryById(categoryId), out CategoryModel? category))
            {
                return category ?? new CategoryModel();
            }

            category = await _context.Categories.FindAsync(categoryId);
            // Kiểm tra null và lưu cache
            if (category != null)
            {
                _cache.Set(CacheKeys.CategoryById(categoryId), category, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new CategoryModel();
            }
            return category;
        }

        //Xóa danh mục
        public async Task<bool> DeleteCategory(int categoryId)
        {
            var categoryExisting = await _context.Categories.FindAsync(categoryId);
            if (categoryExisting == null)
            {
                return false;
            }
            _context.Categories.Remove(categoryExisting);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.AllCategories);
            _cache.Remove(CacheKeys.CategoryById(categoryId));
            return true;
        }

        //Sua danh mucch
        public async Task<CategoryModel> UpdateCategory(CategoryModel category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);

            if (existingCategory == null)
            {
                throw new ArgumentException("Category not found.");
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentException("Category name is required.");            

            existingCategory.CategoryName = category.CategoryName != null ? category.CategoryName : existingCategory.CategoryName;
            existingCategory.Description = category.Description != null ? category.Description : existingCategory.Description;
            existingCategory.Slug = category.Slug != null ? category.Slug : existingCategory.Slug;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            //Set lại cache
            _cache.Set(CacheKeys.CategoryById(category.CategoryId), existingCategory, TimeSpan.FromMinutes(10));
            _cache.Remove(CacheKeys.AllCategories);

            return existingCategory;
        }

        //Check slug
        public async Task<bool> CheckSlug(string slug)
        {
            return await _context.Categories.AnyAsync(x => x.Slug == slug);
        }
    }
}
