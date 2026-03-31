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
    public class WorkTaskRepository : GenericRepository<WorkTask>, IWorkTaskRepository
    {
        public WorkTaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<WorkTask>> GetTasksByGroupIdAsync(int groupId, int? labelId = null, TasksStatus? taskStatus = null, TaskPriority? taskPriority = null)
        {
            return await _context.WorkTask
                .Where(x => x.GroupId == groupId)
                .Where(t => taskStatus == null || t.Status == taskStatus)
                .Where(t => taskPriority == null || t.Priority == taskPriority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<WorkTask?> GetByIdWithLabelsAsync(int taskId)
        {
            return await _context.WorkTask
                .Include(t => t.TaskLabels)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<List<WorkTask>> GetTasksByUserIdAsync(int groupId, int userId, int? labelId = null, TasksStatus? taskStatus = null, TaskPriority? taskPriority = null)
        {
            return await _context.WorkTask
                .Where(t => t.GroupId == groupId)
                .Where(t => t.AssignedTo == userId)
                .Where(t => taskStatus == null || t.Status == taskStatus)
                .Where(t => taskPriority == null || t.Priority == taskPriority)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
