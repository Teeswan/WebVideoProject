using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(Guid id);
        Task AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
    }
}
