using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;

namespace project.Infrastructure.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
