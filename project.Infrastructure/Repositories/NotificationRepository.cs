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
    }
}
