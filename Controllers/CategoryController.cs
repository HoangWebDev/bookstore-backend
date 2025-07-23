using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/category/")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        //Lấy tất cả danh mục
        [HttpGet("all_categories")]        
        public async Task<ActionResult<List<CategoryModel>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategories();
                return Ok(categories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Lấy 1 danh mục
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<CategoryModel>> GetCategoryById(int categoryId)
        {
            try
            {
                if (categoryId <= 0) 
                    return NotFound("Invalid category ID. Must be a positive integer.");

                var category = await _categoryRepository.GetCategoryById(categoryId);
                return Ok(category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Tạo danh mục
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("new_category")]
        public async Task<ActionResult<CategoryModel>> AddNewCategory([FromBody] CategoryModel category)
        {
            try
            {
                if (category == null) return NotFound("Data found");
                var createdCategory = await _categoryRepository.CreateCategory(category); // Lưu kết quả trả về

                return Ok(createdCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Cạp nhật danh mục
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("update_category")]
        public async Task<ActionResult<CategoryModel>> UpdateCategory([FromBody] CategoryModel category)
        {
            try
            {
                if (category.CategoryId <= 0)
                    return NotFound("Invalid category ID. Must be a positive integer.");

                if (category == null) return NotFound("Data found");

                var updatedCategory = await _categoryRepository.UpdateCategory(category);

                return Ok(updatedCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Xóa danh mục
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("delete_category/{categoryId}")]
        public async Task<ActionResult<bool>> DeleteCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0) return NotFound("Invalid category ID. Must be a positive integer.");

                var isDeleted = await _categoryRepository.DeleteCategory(categoryId);

                return Ok("Deleted successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Check slug
        [HttpGet("check-slug/{slug}")]
        public async Task<IActionResult> CheckSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest("Slug is required.");

            var exists = await _categoryRepository.CheckSlug(slug);
            return Ok(new { exists });
        }
    }
}
