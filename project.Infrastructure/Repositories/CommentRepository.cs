using Microsoft.EntityFrameworkCore;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;
using System.Threading.Tasks;

namespace project.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Comment?> GetCommentByTaskIdAsync(int taskId)
        {
            return await _context.Comment
                        .Include(c => c.Task)
                        .Include(c => c.Replies)
                        .ThenInclude(c => c.User)
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(c => c.TaskId == taskId && !c.IsDeleted);
        }

        public async Task<IReadOnlyList<Comment?>> GetCommentsByTaskIdAsync(int taskId)
        {
            return await _context.Comment
                        .Where(c => c.TaskId == taskId && !c.IsDeleted)
                        .Include(c => c.User)
                        .Include(c => c.Replies.Where(r => !r.IsDeleted))
                        .ThenInclude(r => r.User)
                        .AsSplitQuery()
                        .OrderBy(c => c.CreatedAt)
                        .ToListAsync();
        }
    }
}
