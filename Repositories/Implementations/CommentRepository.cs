using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Repositories.Interfaces;

namespace Youtube_Entertainment_Project.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Video)
                .Include(c => c.Replies)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Video)
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
