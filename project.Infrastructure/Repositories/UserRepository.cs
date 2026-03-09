using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<UserApp>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddRangeAsync(IEnumerable<UserApp> users)
        {
            await _context.User.AddRangeAsync(users);
        }

        public async Task<UserApp?> FindByEmailAsync(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserApp?> FindByUserCode(string userCode)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.UserCode == userCode);
        }

        public async Task<string> GetRoleAsync(int id)
        {
            var role = await _context.User
                .Where(u => u.Id == id)
                .Select(u => u.UserRole.ToString())
                .FirstOrDefaultAsync();
            return role ?? string.Empty;

        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.User.AnyAsync(u => u.Email == email);
        }
    }
}
