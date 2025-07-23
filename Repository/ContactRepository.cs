using BookStore.Data;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Utils;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DapperContext _dapperContext;
        public readonly DataContext _context;

        public ContactRepository(DataContext context, DapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<ContactModel?> GetContact(int contactId)
        {
            return await _context.Contacts.FindAsync(contactId);
        }

        public async Task<ContactModel> UpdateContact(ContactModel contact)
        {
            var existingContact = await _context.Contacts.FirstOrDefaultAsync();

            if (existingContact == null)
            {
                // Tạo mới nếu chưa có
                var newContact = new ContactModel
                {
                    Content = contact.Content
                    // có thể thêm các field khác nếu có
                };

                _context.Contacts.Add(newContact);
                await _context.SaveChangesAsync();
                return newContact;
            }
            else
            {
                // Cập nhật nếu đã có
                existingContact.Content = contact.Content ?? existingContact.Content;

                _context.Contacts.Update(existingContact);
                await _context.SaveChangesAsync();
                return existingContact;
            }
        }
    }
}
