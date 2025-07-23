using BookStore.Dto;
using BookStore.Models;

namespace BookStore.IRepository
{
    public interface IUserRepository
    {
        public Task<List<UserModel>> GetAllUsers();
        public Task<UserModel> GetUserById(int userId);
        public Task<UserModel> UpdateUser(UserDto user);
        public Task<bool> DeleteUser(int userId);
    }
}
