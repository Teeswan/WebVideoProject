using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<AppUser>> GetAllAsync();
        Task<AppUser?> GetByIdAsync(Guid id);
        Task AddAsync(AppUser appuser);
        Task UpdateAsync(AppUser appuser);
        Task DeleteAsync(AppUser appuser);
    }
}
