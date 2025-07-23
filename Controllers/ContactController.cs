using BookStore.DTO;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/contact/")]
    public class ContatcController : Controller
    {
        private readonly IContactRepository _contactRepository;

        public ContatcController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        //Lay tat ca contact

        [HttpGet("contact/{contactId}")]
        public async Task<ActionResult<ContactModel>> GetContact(int contactId)
        {
            try
            {
                var contact = await _contactRepository.GetContact(contactId);            
                return Ok(contact);
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

        //Cập nhật contact      
        [HttpPut("update_contact")]
        public async Task<ActionResult<ContactModel>> UpdateContact([FromBody] ContactModel contact)
        {
            try
            {                
                if (contact == null) return NotFound("Contact data is required");

                var contactUpdate = await _contactRepository.UpdateContact(contact);

                return Ok(contactUpdate);
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
    }
}
