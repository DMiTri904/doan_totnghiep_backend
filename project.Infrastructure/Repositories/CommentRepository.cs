using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;

namespace project.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
