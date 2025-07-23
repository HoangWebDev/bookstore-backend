using BookStore.IRepository;
using BookStore.Models;
using BookStore.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/book/")]
    public class BookController : Controller
    {        
        private readonly IBookRepository _bookRepository;
        private readonly CloudinaryService _cloudinaryService;
        public BookController(IBookRepository bookRepository, CloudinaryService cloudinaryService)
        {
            _bookRepository = bookRepository;
            _cloudinaryService = cloudinaryService;
        }

        //Lấy tất cả sách
        [HttpGet("all_books")]        
        public async Task<ActionResult<List<BookModel>>> GetAllBooks()
        {
            try
            {
                var books = await _bookRepository.GetAllBooks();

                return Ok(books);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Lấy 1 sách
        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<BookModel>> GetBookId(int bookId)
        {
            try
            {
                if (bookId <= 0) 
                    return NotFound("Invalid book ID. Must be a positive integer.");

                var book = await _bookRepository.GetBookById(bookId);

                return Ok(book);
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

        //Lấy 1 sách
        [HttpGet("bookbyslug/{slug}")]
        public async Task<ActionResult<BookModel>> GetBookSlug(string slug)
        {
            try
            {
                if (slug == null)
                    return NotFound("Invalid book by lsug. Must be a positive integer.");

                var book = await _bookRepository.GetBookBySlug(slug);

                return Ok(book);
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

        //Thêm sách mới
        [Authorize(Roles = "Admin")]
        [HttpPost("new_book")]        
        public async Task<ActionResult<BookModel>> AddNewBook([FromForm] BookModel book, IFormFile? image)           
        {
            try
            {
                if (book == null) return BadRequest("Data not found");

                // Upload ảnh lên Cloudinary nếu có
                if (image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(image);
                    book.Image = imageUrl; // Lưu URL của ảnh vào DB
                }

                var createdBook = await _bookRepository.CreateBook(book);
                return Ok(createdBook);
            }
            catch ( ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Cập nhật sách
        [Authorize(Roles = "Admin")]
        [HttpPut("update_book")]
        public async Task<ActionResult<BookModel>> UpdateBook([FromForm] BookModel book, IFormFile? image)
        {
            try
            {
                if (book.BookId <= 0) return NotFound("Invalid book ID. Must be a positive integer.");

                if (book == null) return NotFound("Data found");

                // Nếu có ảnh mới, upload lên Cloudinary và cập nhật đường dẫn
                if (image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(image);
                    book.Image = imageUrl;
                }

                var updatedBook = await _bookRepository.UpdateBook(book); // Lưu kết quả trả về

                return Ok(updatedBook);
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

        //Xóa sách
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete_book/{bookId}")]
        public async Task<ActionResult<bool>> DeleteBook(int bookId)
        {
            try
            {
                if (bookId <= 0) return NotFound("Invalid book ID. Must be a positive integer.");

                var isDeleted = await _bookRepository.DeleteBook(bookId);

                return Ok("Deleted successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        //Tìm kiếm sách
        [HttpGet("search_book")]
        public async Task<ActionResult<List<BookModel>>> SearchBook([FromQuery] SearchRequest keyword)
        {
            try
            {
                var books = await _bookRepository.SearchBook(keyword.Keyword);
                return Ok(books);
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
      
        //Lọc sách tổng hợp
        [HttpGet("filter")]
        public async Task<ActionResult<List<BookModel>>> FilterBooks([FromQuery] FilterBooksRequest request)
        {
            try
            {
                var books = await _bookRepository.FilterBooks(
                    request.MinPrice,
                    request.MaxPrice,
                    request.AuthorId,
                    request.PublisherId,
                    request.CategoryId
                );

                return Ok(books);
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

            var exists = await _bookRepository.CheckSlug(slug);
            return Ok(new { exists });
        }
    }
}
