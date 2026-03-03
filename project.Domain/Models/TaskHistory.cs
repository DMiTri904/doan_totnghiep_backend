namespace project.Domain.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ChangedBy { get; set; }
        public TaskStatus OldStatus { get; set; }
        public TaskStatus NewStatus { get; set; }
        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public WorkTask Task { get; set; }
        public UserApp User { get; set; }
    }
}
