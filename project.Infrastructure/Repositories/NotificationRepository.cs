using Microsoft.EntityFrameworkCore;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;

namespace project.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetNotificationByUserId(int userId)
        {
            return await _context.Notification
                        .Where(n => n.UserId == userId)
                        .ToListAsync();
        }

        public async Task<IReadOnlyList<Notification>> GetUnreadNotificationByUserId(int userId)
        {
            return await _context.Notification
                        .Where(n => n.UserId == userId && !n.IsRead)
                        .ToListAsync();
        }
    }
}
