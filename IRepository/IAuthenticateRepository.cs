using BookStore.Models;

namespace BookStore.IRepository
{
    public interface IAuthenticateRepository
    {
        public Task<Dictionary<string, string>> Login(string username, string password);
        public Task<UserModel?> Register(string username, string password, string fullname);

        public UserModel? GetProfile(string token);
    }
}
