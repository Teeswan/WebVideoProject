using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(Guid subscriberUserId, Guid channelId);
        Task AddAsync(Subscription subscription);
        Task DeleteAsync(Subscription subscription);
        Task<Subscription> GetSubscriptionByUsersAsync(Guid subscriberUserId, Guid channelId);
        Task<int> CountByChannelOwnerIdAsync(Guid channelId);
        Task<List<Subscription>> GetSubscriptionsBySubscriberAsync(Guid subscriberUserId);

    }
}
