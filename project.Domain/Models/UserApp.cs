using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class UserApp
    {
        public int Id { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; private set; } 
        public string Email { get; private set; } = string.Empty;
        public string? AvatarUrl { get; private set; } = string.Empty;
        public string? UserCode { get; private set; } = string.Empty;
        public UserRole UserRole { get; private set; }

        public ICollection<GroupMem> GroupMembers => _groupmembers.AsReadOnly();
        public ICollection<WorkTask> AssignedTasks => _assignedtasks.AsReadOnly();
        public ICollection<Comment> Comments => _comments.AsReadOnly();
        public ICollection<TaskHistory> TaskHistories => _taskhistories.AsReadOnly();
        public ICollection<ActivityLog> ActivityLogs => _activitylogs.AsReadOnly();
        public ICollection<Notification> Notifications => _notifications.AsReadOnly();

        private readonly List<GroupMem> _groupmembers = new();
        private readonly List<WorkTask> _assignedtasks = new();
        private readonly List<Comment> _comments = new();
        private readonly List<TaskHistory> _taskhistories = new();
        private readonly List<ActivityLog> _activitylogs = new();
        private readonly List<Notification> _notifications = new();

        private UserApp() { }

        public static UserApp Create(string userName, string email, string passwordHash, string userCode, UserRole userRole)
        {
            if (string.IsNullOrEmpty(userName)) throw new DomainException("Tên không được trống");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email không được trống");
            if (string.IsNullOrWhiteSpace(userCode)) throw new DomainException("Mã sinh viên/giáo viên không được trống");
            if (string.IsNullOrEmpty(userRole.ToString())) throw new DomainException("Role không được để trống");

            return new UserApp
            {
                UserName = userName,
                Email = email,
                UserCode = userCode,
                UserRole = userRole,
                IsActive = true,
                PasswordHash = passwordHash
            };
        }
        public void UpdateAvatarProfile(string? avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }
        public void ChangePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }
    }
}
