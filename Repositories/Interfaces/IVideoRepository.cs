using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetAllAsync();
        Task<Video?> GetByIdAsync(Guid id);
        Task AddAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(Video video);
    }
}
