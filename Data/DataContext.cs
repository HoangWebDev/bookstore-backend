using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        public DbSet<BookModel> Books { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<PublisherModel> Publishers { get; set; }
        public DbSet<ChatModel> Chats { get; set; }        
        public DbSet<ContactModel> Contacts { get; set; }
    }
}
