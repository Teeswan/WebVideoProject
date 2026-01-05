using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<List<PlaylistDto>> GetAllPlaylistsAsync();
        Task<PlaylistDto> GetPlaylistByIdAsync(Guid id);
        Task<PlaylistDto> CreatePlaylistAsync(PlaylistDto dto);
        Task<PlaylistDto> UpdatePlaylistAsync(Guid id, PlaylistDto dto);
        Task DeletePlaylistAsync(Guid id);
        Task AddVideoToPlaylistAsync(Guid playlistId, Guid videoId);
        Task<IEnumerable<PlaylistDto>> GetPlaylistsByUserIdAsync(Guid userId);
        
        }
}
