using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface IChannelRepository
    {
        Task<List<Channel>> GetAllAsync();
        Task<Channel?> GetByIdAsync(Guid id);
        Task AddAsync(Channel channel);
        Task UpdateAsync(Channel channel);
        Task DeleteAsync(Channel channel);
        Task<bool> ExistsByOwnerIdAsync(Guid ownerUserId);
    }
}
