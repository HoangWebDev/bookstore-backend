using BookStore.Data;
using BookStore.IRepository;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Repository
{
    public class AuthenticateRepository : IAuthenticateRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticateRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //Đăng ký
        public async Task<UserModel?> Register(string username, string password, string fullname)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == username))
            {
                return null;
            }            

            var user = new UserModel 
            { 
                UserName = username, 
                PasswordHash = HashPassword(password),
                FullName = fullname, 
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        //Đăng nhập
        public async Task<Dictionary<string, string>> Login(string username, string password)
        {
            var errors = new Dictionary<string, string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
            {
                errors["username"] = "User not found";
                return errors;
            }

            if (!VerifyPassword(password, user.PasswordHash!))
            {
                errors["password"] = "Password is incorrect";
                return errors;
            }

            return new Dictionary<string, string> { { "token", GenerateToken(user) } };
        }

        //Băm Password
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        //Mã hóa Password
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        //Generate Token
        public string GenerateToken(UserModel user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {                
                new Claim("userId", user.UserId.ToString()),  // ID của user
                new Claim("userName", user.UserName!),   // Username
                new Claim("passwordHash", user.PasswordHash!), // Password
                new Claim(ClaimTypes.Role, user.Role!),           // Role
                new Claim("fullName", user.FullName!),    // Full name
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
            };

            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Get profile
        public UserModel? GetProfile(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(token, validations, out var validatedToken);
                var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

                //Lấy user từ token
                claims.TryGetValue(ClaimTypes.Role, out string? role);
if (role == null)
    claims.TryGetValue("role", out role);

                return new UserModel
                {
                    UserId = int.Parse(claims["userId"]),
                    UserName = claims["userName"],
                    PasswordHash = claims["passwordHash"],
                    Role = role,
                    FullName = claims["fullName"]
                };
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
