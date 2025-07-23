using BookStore.DTO;
using BookStore.Models;

namespace BookStore.IRepository
{
    public interface IContactRepository
    {
        public Task<ContactModel?> GetContact(int contactId);

        public Task<ContactModel> UpdateContact(ContactModel contact);
    }
}
