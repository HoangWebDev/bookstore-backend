using BookStore.Dto;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Repository;
using BookStore.Request;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //Lay tat ca user
        [HttpGet("all_users")]
        public async Task<ActionResult<List<UserModel>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();            
                return Ok(users);
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

        //Lấy 1 user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<UserModel>> GetUserById(int userId)
        {
            try
            {
                if (userId  <= 0)
                    return NotFound("Invalid publisher ID. Must be a positive integer.");

                var publisher = await _userRepository.GetUserById(userId );
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

        //Cập nhật user        
        [HttpPut("update_user")]
        public async Task<ActionResult<UserModel>> UpdateUser([FromBody] UserDto user)
        {
            try
            {
                if (user.UserId <= 0)
                    return NotFound("Invalid publisher ID. Must be a positive integer.");

                if (user == null) return NotFound("Data found");

                var updatedUser = await _userRepository.UpdateUser(user);

                return Ok(updatedUser);
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

        //Xóa user
        [HttpDelete("delete_user/{userId}")]
        public async Task<ActionResult<bool>> DeleteUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return NotFound("Invalid publisher ID. Must be a positive integer.");

                var isDeleted = await _userRepository.DeleteUser(userId);

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
    }
}
