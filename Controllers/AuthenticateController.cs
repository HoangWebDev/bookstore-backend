using BookStore.IRepository;
using BookStore.Models;
using BookStore.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/authen/")]
    public class AuthenticateController : Controller
    {
        private readonly IAuthenticateRepository _authenticate;

        public AuthenticateController(IAuthenticateRepository authenticate)
        {
            _authenticate = authenticate;
        }
        
        //Đăng ký
        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register([FromBody] RegisterRequest registerRequest)
        {
            var register = await _authenticate.Register(registerRequest.UserName!, registerRequest.Password!, registerRequest.FullName!);

            return register == null ? BadRequest("User already exists") : Ok(register);
        }

        //Đăng nhập
        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var result = await _authenticate.Login(loginRequest.UserName!, loginRequest.Password!);

                if (result.ContainsKey("username") || result.ContainsKey("password"))
                {
                    return BadRequest(result);
                }

                var token = result["token"];
                var user = _authenticate.GetProfile(token);

                return Ok(new
                {
                    token = token,
                    user = user
                });
            }            
            catch (Exception)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi từ máy chủ. Vui lòng thử lại sau." });
            }
        }                
    }
}
