using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .Include(t => t.VideoTags)
                .ThenInclude(vt => vt.Video)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags
                .Include(t => t.VideoTags)
                .ThenInclude(vt => vt.Video)
                .FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
