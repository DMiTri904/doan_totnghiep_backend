using project.Domain.Models;

namespace project.Presentation.Models
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedTo { get; set; }
        public List<int>? LabelIds { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedTo { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        public TasksStatus NewStatus { get; set; }
        public string? Note { get; set; }
    }

    public class AssignTaskRequest
    {
        public int? AssignedTo { get; set; } // null = unassign
    }

    public class AddTaskLabelRequest
    {
        public int LabelId { get; set; }
    }

    public class RemoveTaskLabelRequest
    {
        public int LabelId { get; set; }
    }
}
