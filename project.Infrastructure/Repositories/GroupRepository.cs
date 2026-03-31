using Microsoft.EntityFrameworkCore;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;
using System.Text.RegularExpressions;

namespace project.Infrastructure.Repositories
{
    public class GroupRepository : GenericRepository<Groups>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Groups>> GetAllGroupsByUserIdAsync(int userId)
        {
            var x = await _context.Groups
               .Include(g => g.Members)
               .Where(g => g.Members.Any(m => m.UserId == userId && m.IsActive))
               .ToListAsync();
            return x;
        }

        public async Task<Groups?> GetByIdWithDetailAsync(int id)
        {
            return await _context.Groups.
                        Include(c => c.Members).
                        Include(c => c.Tasks).
                        AsSplitQuery().
                        FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Groups?> GetByIdWithMemberAsync(int id)
        {
            return await _context.Groups.
                        Include(c => c.Members).
                        ThenInclude(c => c.User).
                        AsSplitQuery().
                        FirstOrDefaultAsync(c => c.Id == id);
        }  
    }
}
