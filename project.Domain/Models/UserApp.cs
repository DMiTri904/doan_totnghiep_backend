namespace project.Domain.Models
{
    public class UserApp
    {
        public int Id { get; private set; }
        public string IdentityId { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? AvatarUrl { get; private set; } = string.Empty;
        public string? StudentCode { get; private set; } = string.Empty;
        public UserRole UserRole { get; private set; }

        public ICollection<GroupMember> GroupMembers => _groupmembers.AsReadOnly();
        public ICollection<WorkTask> AssignedTasks => _assignedtasks.AsReadOnly();
        public ICollection<Comment> Comments => _comments.AsReadOnly();
        public ICollection<TaskHistory> TaskHistories => _taskhistories.AsReadOnly();
        public ICollection<ActivityLog> ActivityLogs => _activitylogs.AsReadOnly();
        public ICollection<Notification> Notifications => _notifications.AsReadOnly();

        private readonly List<GroupMember> _groupmembers = new();
        private readonly List<WorkTask> _assignedtasks = new();
        private readonly List<Comment> _comments = new();
        private readonly List<TaskHistory> _taskhistories = new();
        private readonly List<ActivityLog> _activitylogs = new();
        private readonly List<Notification> _notifications = new();

        private UserApp() { }

        public UserApp(int id, string userName, string email, string? avatarUrl, string studentCode, UserRole userRole)
        {
            Id = id;
            UserName = userName;
            Email = email;
            AvatarUrl = avatarUrl;
            StudentCode = studentCode;
            UserRole = userRole;
        }
        public void UpdateProfile(string userName, string? avatarUrl)
        {
            UserName = userName;
            AvatarUrl = avatarUrl;
        }
    }
}
