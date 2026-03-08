using project.Domain.Exceptions;
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


    public class Groups
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        public string SubjectOrProjectName { get; private set; } = string.Empty;
        public int CreatedBy { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public UserApp Creator { get; private set; }
        public ICollection<GroupMem> Members => _members.AsReadOnly();
        public ICollection<WorkTask> Tasks => _worktask.AsReadOnly();
        public ICollection<ActivityLog> ActivityLogs => _activityLogs.AsReadOnly();
        public ICollection<Report> Reports => _reports.AsReadOnly();

        private readonly List<GroupMem> _members = new();
        private readonly List<WorkTask> _worktask = new();
        private readonly List<ActivityLog> _activityLogs = new();
        private readonly List<Report> _reports = new();

        private Groups() { }

        public Groups(string name, string? description, string subjectOrProjectName, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tên nhóm không thể trống");
            if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Mô tả nhóm không thể trống");
            if (string.IsNullOrWhiteSpace(subjectOrProjectName)) throw new DomainException("Tên môn học hoặc dự án không thể trống");

            Name = name;
            Description = description;
            SubjectOrProjectName = subjectOrProjectName;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
        // Member management
        public void UpdateDetails(string name, string? description, string subjectOrProjectName)
        {
            if (!IsActive) throw new DomainException("Không thể cập nhật nhóm đã bị vô hiệu hóa");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tên nhóm không thể để trống");

            Name = name;
            Description = description;
            SubjectOrProjectName = subjectOrProjectName;
        }
        public void AddMember(GroupMem member)
        {
            var existing = _members.FirstOrDefault(m => m.UserId == member.UserId);

            if (existing != null)
            {
                if (existing.IsActive) throw new DomainException("Người dùng đã là thành viên");
                existing.Rejoin(); // reactivate thay vì tạo mới
                return;
            }
            _members.Add(member);
        }
        public GroupMem? FindMember(int userId)
        {
            return _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
        }
        public void DeactivateGroup()
        {
            IsActive = false;
        }
        public void ReactiveGroup()
        {
            IsActive = true;
        }
        public void AddTask(WorkTask task)
        {
            if(!IsActive) throw new DomainException("Không thể thêm công việc vào nhóm đã bị vô hiệu hóa");
            _worktask.Add(task);
        }
        // Activity log management
        public void ContributeActivityLog(ActivityLog log)
        {
            if (!IsActive) throw new DomainException("Không thể ghi nhật ký hoạt động cho nhóm đã bị vô hiệu hóa");
            _activityLogs.Add(log);
        }
        // Report management
        public void AddReport(Report report)
        {
            if (!IsActive) throw new DomainException("Không thể thêm báo cáo vào nhóm đã bị vô hiệu hóa");
            _reports.Add(report);
        }
        public void RemoveReport(int reportId)
        {
            if (!IsActive) throw new DomainException("Không thể xóa báo cáo khỏi nhóm đã bị vô hiệu hóa");
            var report = _reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null) throw new DomainException("Báo cáo không tồn tại trong nhóm");
            _reports.Remove(report);
        }

        public int ActiveMemberCount()
        {
            return _members.Count(m => m.IsActive);
        }
        public int PendingTaskCount()
        {
            return _worktask.Count(m => m.Status != TaskStatus.Done);
        }

    }
}
