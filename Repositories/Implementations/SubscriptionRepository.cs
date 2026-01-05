using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(Guid subscriberUserId, Guid channelOwnerUserId)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .FirstOrDefaultAsync(s => s.SubscriberUserId == subscriberUserId
                                       && s.ChannelOwnerUserId == channelOwnerUserId);
        }

        public async Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
        public async Task<Subscription?> GetSubscriptionByUsersAsync(Guid subscriberUserId, Guid channelOwnerUserId)
        {
            return await _context.Subscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SubscriberUserId == subscriberUserId
                                       && s.ChannelOwnerUserId == channelOwnerUserId);
        }

        public async Task<int> CountByChannelOwnerIdAsync(Guid channelOwnerUserId)
        {
            return await _context.Subscriptions
                .CountAsync(s => s.ChannelOwnerUserId == channelOwnerUserId);
        }
        public async Task<List<Subscription>> GetSubscriptionsBySubscriberAsync(Guid subscriberUserId)
        {
            return await _context.Subscriptions
                .Where(s => s.SubscriberUserId == subscriberUserId)
                .ToListAsync();
        }
    }
}
