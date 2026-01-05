using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AppUser>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Channels)
                .Include(u => u.Playlists)
                .ToListAsync();
        }
        public async Task<AppUser?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Channels)
                .Include(u => u.Playlists)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task AddAsync(AppUser appuser)
        {
            await _context.Users.AddAsync(appuser);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(AppUser appuser)
        {
            _context.Users.Update(appuser);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(AppUser appuser)
        {
            _context.Users.Remove(appuser);
            await _context.SaveChangesAsync();
        }
    }
}
