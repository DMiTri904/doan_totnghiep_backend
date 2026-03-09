using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;

namespace project.Infrastructure.Repositories
{
    public class GroupRepository : GenericRepository<Groups>, IGroupRepository
    {
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
