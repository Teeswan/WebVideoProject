using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Video>> GetAllAsync()
        {
            return await _context.Videos
                .Include(v => v.Channel)
                .Include(v => v.Category)
                .Include(v => v.Thumbnails)
                .Include(v => v.Comments)
                .Include(v => v.Likes)
                .Include(v => v.VideoTags)
                    .ThenInclude(vt => vt.Tag)
                .ToListAsync();
        }

        public async Task<Video?> GetByIdAsync(Guid id)
        {
            return await _context.Videos
                .Include(v => v.Channel)
                .Include(v => v.Category)
                .Include(v => v.Thumbnails)
                .Include(v => v.Comments)
                .Include(v => v.Likes)
                .Include(v => v.VideoTags)
                    .ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync(v => v.VideoId == id);
        }

        public async Task AddAsync(Video video)
        {
            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Video video)
        {
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Video video)
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }
    }
}
