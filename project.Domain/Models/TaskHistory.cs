using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class TaskHistory
    {
        public int Id { get; private set; }
        public int TaskId { get; private set; }
        public int ChangedBy { get; private set; }
        public TaskStatus OldStatus { get; private set; }
        public TaskStatus NewStatus { get; private set; }
        public string? Note { get; private set; }
        public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;
        public WorkTask Task { get; private set; }
        public UserApp User { get; private set; }

        private TaskHistory() { }

        // Chỉ được tạo qua factory — không ai tạo lịch sử giả
        public static TaskHistory Create(int taskId, int changedBy,
            TaskStatus oldStatus, TaskStatus newStatus, string? note = null)
        {
            if (oldStatus == newStatus)
                throw new DomainException("Trạng thái mới phải khác trạng thái cũ");

            return new TaskHistory
            {
                TaskId = taskId,
                ChangedBy = changedBy,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Note = note,
                ChangedAt = DateTime.UtcNow
            };
        }

    }
}
