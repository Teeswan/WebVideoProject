
using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface IChannelService
    {
        Task<List<ChannelDto>> GetAllChannelsAsync();
        Task<ChannelDto> GetChannelByIdAsync(Guid id);
        Task<ChannelDto> GetChannelByUserIdAsync(Guid userId);
        Task<ChannelDto> CreateChannelAsync(ChannelDto dto, Guid ownerUserId);
        Task<ChannelDto> UpdateChannelAsync(Guid id, ChannelDto dto, Guid currentUserId);
        Task DeleteChannelAsync(Guid id, Guid currentUserId);
        Task<bool> ChannelExistsByOwnerIdAsync(Guid ownerUserId);
        Task<IEnumerable<ChannelDto>> SearchChannelsAsync(string searchTerm);
    }
}
