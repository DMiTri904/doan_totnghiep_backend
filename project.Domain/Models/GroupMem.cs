using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class GroupMem
    {
        public int Id { get; private set; }
        public int GroupId { get; private set; }
        public int UserId { get; private set; } 
        public bool IsActive { get; private set; } = true;
        public GroupMemberRole Role { get; private set; } = GroupMemberRole.Member;
        public DateTime JoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }
        public Groups Group { get; private set; }
        public UserApp User { get; private set; }


        private GroupMem() { }

        public static GroupMem Create(int groupId, int userId, GroupMemberRole role = GroupMemberRole.Member)
        {
            return new GroupMem
            {
                GroupId = groupId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            };
        }
        public void Rejoin()
        {
            if (IsActive) throw new DomainException("Thành viên đang hoạt động");
            IsActive = true;
            LeftAt = null;
            JoinedAt = DateTime.UtcNow;
        }
        public void Leave()
        {
            if (!IsActive) throw new DomainException("Thành viên đã rời nhóm");
            LeftAt = DateTime.UtcNow;
            IsActive = false;
        }

        public void PromoteTo(GroupMemberRole newRole)
        {
            if (!IsActive) throw new DomainException("Thành viên không còn trong nhóm");
            if (Role == newRole) throw new DomainException($"Thành viên đã có role {newRole} rồi");

            Role = newRole;
        }
        public bool IsLeader() => Role == GroupMemberRole.Leader;
        public bool CanManageTasks() => Role is GroupMemberRole.Leader;
    }
}
