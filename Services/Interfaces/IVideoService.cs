using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface IVideoService
    {
        Task<List<VideoDto>> GetAllVideosAsync();
        Task<VideoDto> GetVideoByIdAsync(Guid id);
        Task<VideoDto> CreateVideoAsync(CreateVideoDto dto, Guid currentUserId);
        Task<VideoDto> UpdateVideoAsync(Guid id, UpdateVideoDto dto, Guid currentUserId);
        Task DeleteVideoAsync(Guid id, Guid currentUserId);
        Task UpdateVideoViewsAsync(Guid videoId, int newViewCount);
        Task<bool> HasUserLikedAsync(Guid videoId, Guid userId);
        Task<bool> ToggleLikeAsync(Guid videoId, Guid userId);
        Task<int> GetLikeCountAsync(Guid videoId);
        Task<long> GetTotalLikesForChannelAsync(Guid channelId);
        Task<IEnumerable<VideoDto>> SearchVideosAsync(string searchTerm, Guid? currentUserId);

    }
}
