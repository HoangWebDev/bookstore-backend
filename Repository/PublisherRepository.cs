using BookStore.Data;
using BookStore.Helper;
using BookStore.IRepository;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BookStore.Repository
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly DataContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IMemoryCache _cache;

        public PublisherRepository(DataContext context, DapperContext dapperContext, IMemoryCache cache)
        {
            _context = context;
            _dapperContext = dapperContext;
            _cache = cache;
        }

        //Tạo nhà sản xuất
        public async Task<PublisherModel> CreatePublisher(PublisherModel publisher)
        {
            if (string.IsNullOrWhiteSpace(publisher.PublisherName))
                throw new ArgumentException("Publisher name is required.");

            if (await _context.Publishers.AnyAsync(x => x.PublisherName == publisher.PublisherName))
            {
                throw new ArgumentException("Publisher name already exists.");
            }            

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            //Xóa cache cũ
            _cache.Remove(CacheKeys.AllPublishers);
            return publisher;
        }

        //Lấy tất cả nhà sản xuất
        public async Task<List<PublisherModel>> GetAllPublishers()
        {
            if (_cache.TryGetValue(CacheKeys.AllPublishers, out List<PublisherModel>? publishers))
            {
                return publishers ?? new List<PublisherModel>();
            }

            publishers = await _context.Publishers.ToListAsync();
            //Lưu cache
            if (publishers != null)
            {
                _cache.Set(CacheKeys.AllCategories, publishers, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new List<PublisherModel>();
            }
            return publishers;
        }

        //Lấy nhà sx theo id
        public async Task<PublisherModel> GetPublisherById(int publisherId)
        {
            if (_cache.TryGetValue(CacheKeys.PublisherById(publisherId), out PublisherModel? publisher))
            {
                return publisher ?? new PublisherModel();
            }

            publisher = await _context.Publishers.FindAsync(publisherId);
            // Kiểm tra null và lưu cache
            if (publisher != null)
            {
                _cache.Set(CacheKeys.PublisherById(publisherId), publisher, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new PublisherModel();
            }
            return publisher;
        }

        //Xóa nhà sx
        public async Task<bool> DeletePublisher(int publisherId)
        {
            var publisherExisting = await _context.Publishers.FindAsync(publisherId);
            if (publisherExisting == null)
            {
                return false;
            }
            _context.Publishers.Remove(publisherExisting);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.AllPublishers);
            _cache.Remove(CacheKeys.PublisherById(publisherId));
            return true;
        }

        //Cập nhật nhà sx
        public async Task<PublisherModel> UpdatePublisher(PublisherModel publisher)
        {
            var existingPublisher = await _context.Publishers.FindAsync(publisher.PublisherId);

            if (existingPublisher == null)
            {
                throw new ArgumentException("Publisher not found.");
            }

            if (string.IsNullOrWhiteSpace(publisher.PublisherName))
                throw new ArgumentException("Publisher name is required.");          

            existingPublisher.PublisherName = publisher.PublisherName != null ? publisher.PublisherName : existingPublisher.PublisherName;
            existingPublisher.Address = publisher.Address != null ? publisher.Address : existingPublisher.Address;
            existingPublisher.Slug = publisher.Slug != null ? publisher.Slug : existingPublisher.Slug;

            _context.Publishers.Update(existingPublisher);
            await _context.SaveChangesAsync();

            //Set lại cache
            _cache.Set(CacheKeys.PublisherById(publisher.PublisherId), existingPublisher, TimeSpan.FromMinutes(10));
            _cache.Remove(CacheKeys.AllPublishers);

            return existingPublisher;
        }

        //Check slug
        public async Task<bool> CheckSlug (string slug)
        {
            return await _context.Publishers.AnyAsync(x => x.Slug == slug);
        }
    }
}
