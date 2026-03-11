using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class WorkTask
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        public TasksStatus Status { get; private set; } = TasksStatus.ToDo;
        public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
        public DateTime? StartDate { get; private set; }
        public DateTime? DueDate { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;


        public int GroupId { get; private set; }
        public Groups Groups { get; private set; }
        
        public int? AssignedTo { get; private set; }
        public UserApp? Assignee { get; private set; }

        public UserApp Creator { get; private set; }
        public int CreatedBy { get; private set; }


        public ICollection<Comment> Comments => _comments.AsReadOnly();
        public ICollection<TaskHistory> TaskHistories => _histories.AsReadOnly();
        public ICollection<TaskLabel> TaskLabels => _labels.AsReadOnly();

        private List<Comment> _comments = new List<Comment>();
        private List<TaskHistory> _histories = new List<TaskHistory>();
        private List<TaskLabel> _labels = new List<TaskLabel>();

        private WorkTask() { }
        public static WorkTask Create(int groupId, string title, int createdBy, TaskPriority priority = TaskPriority.Medium)
        {
            if (string.IsNullOrEmpty(title)) throw new DomainException("Tên tiêu đề không thể để trống");

            return new WorkTask
            {
                GroupId = groupId,
                Title = title,
                CreatedBy = createdBy,
                Priority = priority,
                Status = TasksStatus.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow

            };
        }

        public void Assign(int userId)
        {
            if(Status == TasksStatus.Done) throw new DomainException($"Không thể bắt đầu với trạng thái {Status}");

            AssignedTo = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Start()
        {
            if(Status != TasksStatus.ToDo) throw new DomainException("Chỉ có thể bắt đầu công việc ở trạng thái 'To Do'");

            Status = TasksStatus.InProgress;
            StartDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if(Status != TasksStatus.InProgress) throw new DomainException("Chỉ có thể hoàn thành công việc ở trạng thái 'In Progress'");

            Status = TasksStatus.Done;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reopen()
        {
            if (Status != TasksStatus.Done) throw new DomainException("Chỉ có thể mở lại nếu nhiệm vụ ở trạng thái 'Complete' ");

            Status = TasksStatus.InProgress;
            CompletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string title, string? description, TaskPriority priority)
        {
            if(string.IsNullOrEmpty(title)) throw new DomainException("Tên tiêu đề không thể để trống");

            if(Status == TasksStatus.Done) throw new DomainException("Không thể cập nhật chi tiết khi công việc đã hoàn thành");

            Title = title;
            Description = description;
            Priority = priority;
            UpdatedAt = DateTime.UtcNow;
        }

        public void setDueDate(DateTime dueDate)
        {
            if(dueDate < DateTime.UtcNow) throw new DomainException("Ngày đến hạn phải là ngày trong tương lai");

            DueDate = dueDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOverdue() => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != TasksStatus.Done;
        public bool IsAssigned() => AssignedTo.HasValue;
        public TimeSpan? Duration => CompletedAt.HasValue && StartDate.HasValue ? CompletedAt.Value - StartDate.Value : null;

    }
}
