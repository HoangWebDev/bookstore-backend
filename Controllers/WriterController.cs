using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiWriterController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public AiWriterController( GeminiService geminiService)
        {
            // Chỉ định đường dẫn đến file JSON Service Account
            _geminiService = geminiService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] PromptRequest request) // dùng PromptRequest từ BookStore.Request
        {            
            try
            {           
                var result = await _geminiService.GenerateContentAsync(request);
                return Ok(new { content = result });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with Gemini API.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }   
}