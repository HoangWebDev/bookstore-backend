using BookStore.Data;
using BookStore.Dto;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BookStore.Repository
{
    public class UserRepository : IUserRepository
    {

        public readonly DataContext _context;
        public readonly DapperContext _dapperContext;
        public readonly IMemoryCache _cache;

        public UserRepository(DataContext context, DapperContext dapperContext, IMemoryCache cache)
        {
            _context = context;
            _dapperContext = dapperContext;
            _cache = cache;
        }

        //Lay tat ca user
        public async Task<List<UserModel>> GetAllUsers()
        {
            if (_cache.TryGetValue(CacheKeys.AllUsers, out List<UserModel>? users))
            {
                return users ?? new List<UserModel>();
            }

            users = await _context.Users.ToListAsync();

            //Kiểm tra null và lưu cache
            if (users != null)
            {
                _cache.Set(CacheKeys.AllUsers, users, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new List<UserModel>();
            }            
            return users;
        }

        //Lay user theo id
        public async Task<UserModel> GetUserById(int userId)
        {
            if (_cache.TryGetValue(CacheKeys.UserById(userId), out UserModel? user))
            {
                return user ?? new UserModel();
            }

            user = await _context.Users.FindAsync(userId);

            // Kiểm tra null và lưu cache
            if (user != null)
            {
                _cache.Set(CacheKeys.UserById(userId), user, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new UserModel();
            }
            return user;
        }

        //Cập nhật user
        public async Task<UserModel> UpdateUser(UserDto user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);

            if (existingUser == null)
                throw new ArgumentException("User not found.");

            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new ArgumentException("User name is required.");

            existingUser.UserName = user.UserName ?? existingUser.UserName;
            existingUser.PasswordHash = user.PasswordHash != null
                ? HashPassword(user.PasswordHash)
                : existingUser.PasswordHash;
            existingUser.FullName = user.FullName ?? existingUser.FullName;
            existingUser.Role = user.Role ?? existingUser.Role;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            // Set cache
            _cache.Set(CacheKeys.UserById(user.UserId), existingUser, TimeSpan.FromMinutes(10));
            _cache.Remove(CacheKeys.AllUsers);

            return existingUser; // ✅ Sửa chỗ này
        }

        //Xóa user
        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            //Xóa cache
            _cache.Remove(CacheKeys.AllUsers);
            _cache.Remove(CacheKeys.UserById(userId));
            return true;
        }

        //Bam Password
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
