using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Playlist>> GetAllAsync()
        {
            return await _context.Playlists
                .Include(p => p.Owner)              
                .Include(p => p.Videos)            
                    .ThenInclude(pv => pv.Video)   
                .ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(Guid id)
        {
            return await _context.Playlists.Include(p => p.Owner)
                                           .Include(p => p.Videos)
                                           .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }

        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }
    }
}
