using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Domain.Models
{
    public enum GroupMemberRole { Leader, Member }
    public enum TaskStatus { ToDo, InProgress, Done, Cancelled }
    public enum UserRole { Student, Teacher, Admin }
    public enum TaskPriority { Low, Medium, High }
    public enum ActionType { CreateTask, UpdateTask, CompleteTask, Comment, JoinGroup }
    public enum ReportType { Draft, Generating, Completed, Failed }


    public class Group
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        public string SubjectOrProjectName { get; private set; } = string.Empty;
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public UserApp Creator { get; private set; }
        public ICollection<GroupMember> Members => _members.AsReadOnly();
        public ICollection<WorkTask> Tasks => _worktask.AsReadOnly();
        public ICollection<ActivityLog> ActivityLogs => _activityLogs.AsReadOnly();
        public ICollection<Report> Reports => _reports.AsReadOnly();

        private readonly List<GroupMember> _members = new();
        private readonly List<WorkTask> _worktask = new();
        private readonly List<ActivityLog> _activityLogs = new();
        private readonly List<Report> _reports = new();

        private Group() { }

        public Group(string name, string? description, string subjectOrProjectName, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên nhóm không thể trống");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Mô tả nhóm không thể trống");
            if (string.IsNullOrWhiteSpace(subjectOrProjectName)) throw new ArgumentException("Tên môn học hoặc dự án không thể trống");
            if (string.IsNullOrWhiteSpace(createdBy)) throw new ArgumentException("Người tạo nhóm không thể trống");

            Name = name;
            Description = description;
            SubjectOrProjectName = subjectOrProjectName;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
        // Member management
        public void AddMember(GroupMember member)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể thêm thành viên vào nhóm đã bị vô hiệu hóa");

            if (_members.Any(m => m.UserId == member.UserId)) throw new InvalidOperationException("Người dùng đã là thành viên của nhóm");

            _members.Add(member);
        }
        public void RemoveMember(string userId)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể xóa thành viên khỏi nhóm đã bị vô hiệu hóa");

            var member = _members.FirstOrDefault(m => m.UserId == userId);

            if (member == null) throw new InvalidOperationException("Người dùng không phải là thành viên của nhóm");

            _members.Remove(member);
        }
        public void DeactivateGroup()
        {
            IsActive = false;
        }
        // Task management
        public void AddTask(WorkTask task)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể thêm công việc vào nhóm đã bị vô hiệu hóa");
            _worktask.Add(task);
        }
        public void RemoveTask(int taskId)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể xóa công việc khỏi nhóm đã bị vô hiệu hóa");

            var task = _worktask.FirstOrDefault(t => t.Id == taskId);

            if (task == null) throw new InvalidOperationException("Công việc không tồn tại trong nhóm");

            _worktask.Remove(task);
        }
        public void ClearTask()
        {
            _worktask.Clear();
        }

        // Activity log management
        public void ContributeActivityLog(ActivityLog log)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể ghi nhật ký hoạt động cho nhóm đã bị vô hiệu hóa");
            _activityLogs.Add(log);
        }
        // Report management
        public void AddReport(Report report)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể thêm báo cáo vào nhóm đã bị vô hiệu hóa");
            _reports.Add(report);
        }
        public void RemoveReport(int reportId)
        {
            if (!IsActive) throw new InvalidOperationException("Không thể xóa báo cáo khỏi nhóm đã bị vô hiệu hóa");
            var report = _reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null) throw new InvalidOperationException("Báo cáo không tồn tại trong nhóm");
            _reports.Remove(report);
        }
    }
}
