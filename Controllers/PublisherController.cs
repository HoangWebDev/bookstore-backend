using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/publisher/")]
    public class PublisherController : Controller
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        //Lấy tất cả danh mục
        [HttpGet("all_publishers")]        
        public async Task<ActionResult<List<PublisherModel>>> GetAllPublishers()
        {
            try 
            {
                var categories = await _publisherRepository.GetAllPublishers();
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
        [HttpGet("publisher/{publisherId}")]
        public async Task<ActionResult<PublisherModel>> GetPublisherById(int publisherId)
        {
            try
            {
                if (publisherId <= 0) 
                    return NotFound("Invalid publisher ID. Must be a positive integer.");

                var publisher = await _publisherRepository.GetPublisherById(publisherId);
                return Ok(publisher);
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
        [HttpPost("new_publisher")]
        public async Task<ActionResult<PublisherModel>> AddNewPublisher([FromBody] PublisherModel publisher)
        {
            try
            {
                if (publisher == null) return NotFound("Data found");
                var createdPublisher = await _publisherRepository.CreatePublisher(publisher); // Lưu kết quả trả về

                return Ok(createdPublisher);
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
        [HttpPut("update_publisher")]
        public async Task<ActionResult<PublisherModel>> UpdatePublisher([FromBody] PublisherModel publisher)
        {
            try
            {
                if (publisher.PublisherId <= 0)
                    return NotFound("Invalid publisher ID. Must be a positive integer.");

                if (publisher == null) return NotFound("Data found");

                var updatedPublisher = await _publisherRepository.UpdatePublisher(publisher);

                return Ok(updatedPublisher);
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
        [HttpDelete("delete_publisher/{publisherId}")]
        public async Task<ActionResult<bool>> DeletePublisher(int publisherId)
        {
            try
            {
                if (publisherId <= 0) return NotFound("Invalid publisher ID. Must be a positive integer.");

                var isDeleted = await _publisherRepository.DeletePublisher(publisherId);

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

            var exists = await _publisherRepository.CheckSlug(slug);
            return Ok(new { exists });
        }
    }
}
