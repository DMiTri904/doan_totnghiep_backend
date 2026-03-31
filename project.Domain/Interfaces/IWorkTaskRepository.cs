using project.Domain.Models;

namespace project.Domain.Interfaces
{
    public interface IWorkTaskRepository : IGenericRepository<WorkTask>
    {
        Task<List<WorkTask>> GetTasksByGroupIdAsync(int groupId, int? labelId = null, TasksStatus? taskStatus = null, TaskPriority? taskPriority = null);
        Task<WorkTask?> GetByIdWithLabelsAsync(int taskId);
        Task<List<WorkTask>> GetTasksByUserIdAsync(int groupId, int userId, int? labelId = null, TasksStatus? taskStatus = null, TaskPriority? taskPriority = null);
    }
}
