using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetAllAsync();
        Task<Playlist?> GetByIdAsync(Guid id);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(Playlist playlist);
    }
}
