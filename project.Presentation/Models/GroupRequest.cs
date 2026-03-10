using project.Domain.Models;

namespace project.Presentation.Models
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectOrProjectName { get; set; } = string.Empty;
    }
    public class UpdateGroupRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? SubjectOrProjectName { get; set; }
    }

    public class AddMemberRequest
    {
        public int UserId { get; set; }
        public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;
    }

    public class UpdateMemberRoleRequest
    {
        public int UserId { get; set; }
        public GroupMemberRole NewRole { get; set; }
    }

    public class RemoveMemberRequest
    {
        public int UserId { get; set; }
    }

}
