using BookStore.Models;

namespace BookStore.IRepository
{
    public interface IPublisherRepository
    {
        public Task<List<PublisherModel>> GetAllPublishers();
        public Task<PublisherModel> GetPublisherById(int publisherId);
        public Task<PublisherModel> CreatePublisher(PublisherModel publisher);
        public Task<PublisherModel> UpdatePublisher(PublisherModel publisher);
        public Task<bool> DeletePublisher(int publisherId);
        public Task<bool> CheckSlug(string slug);
    }
}
