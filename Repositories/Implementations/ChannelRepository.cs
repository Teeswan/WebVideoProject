using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly ApplicationDbContext _context;

        public ChannelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Channel>> GetAllAsync()
        {
            return await _context.Channels.Include(c => c.Owner).ToListAsync();
        }

        public async Task<Channel?> GetByIdAsync(Guid id)
        {
            return await _context.Channels.Include(c => c.Owner)
                                          .FirstOrDefaultAsync(c => c.ChannelId == id);
        }

        public async Task AddAsync(Channel channel)
        {
            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Channel channel)
        {
            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Channel channel)
        {
            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsByOwnerIdAsync(Guid ownerUserId)
        {
            return await _context.Channels
                .AnyAsync(c => c.OwnerUserId == ownerUserId);
        }
    }
}
