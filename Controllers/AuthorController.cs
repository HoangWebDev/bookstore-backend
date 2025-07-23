using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/author/")]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        //Lấy tất cả danh mục
        [HttpGet("all_authors")]        
        public async Task<ActionResult<List<AuthorModel>>> GetAllAuthors()
        {
            try
            {
                var authors = await _authorRepository.GetAllAuthors();
                return Ok(authors);
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
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<AuthorModel>> GetAuthorById(int authorId)
        {
            try
            {
                if (authorId <= 0) 
                    return NotFound("Invalid author ID. Must be a positive integer.");

                var author = await _authorRepository.GetAuthorById(authorId);
                return Ok(author);
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
        [Authorize(Roles = "Admin")]
        [HttpPost("new_author")]
        public async Task<ActionResult<AuthorModel>> AddNewAuthor([FromBody] AuthorModel author)
        {
            try 
            {
                if (author == null) return NotFound("Data found");
                var createdAuthor = await _authorRepository.CreateAuthor(author); // Lưu kết quả trả về
                return Ok(createdAuthor);
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
        [Authorize(Roles = "Admin")]
        [HttpPut("update_author")]
        public async Task<ActionResult<AuthorModel>> UpdateAuthor([FromBody] AuthorModel author)
        {
            try
            {

                if (author.AuthorId <= 0)
                    return NotFound("Invalid author ID. Must be a positive integer.");
                if (author == null) return NotFound("Data found");

                var updatedAuthor = await _authorRepository.UpdateAuthor(author);
                return Ok(updatedAuthor);
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete_author/{authorId}")]
        public async Task<ActionResult<bool>> DeleteAuthor(int authorId)
        {
            try
            {
                if (authorId <= 0) 
                    return NotFound("Invalid author ID. Must be a positive integer.");

                var isDeleted = await _authorRepository.DeleteAuthor(authorId);

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

            var exists = await _authorRepository.CheckSlug(slug);
            return Ok(new { exists });
        }
    }
}
