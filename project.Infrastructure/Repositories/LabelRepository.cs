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
    public class LabelRepository : GenericRepository<Label>, ILabelRepository
    {
        public LabelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Label>> GetByGroupIdAsync(int groupId, int labelId)
        {
            return await _context.Label
                .Where(c => c.GroupId == groupId)
                .Where(c => labelId == null || c.TaskLabels.Any(tl => tl.LabelId == labelId))
                .ToListAsync();
        }
    }
}
