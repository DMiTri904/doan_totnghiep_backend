using project.Application.ModelsDto.DomainModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.ModelsDto
{
    public class GroupDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SubjectOrProjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? GithubRepoUrl { get; set; }
        public int ActiveMemberCount { get; set; }
        public int PendingTaskCount { get; set; }
        public int TotalTaskCount { get; set; }
    }
    public class GroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SubjectOrProjectName { get; set; } = string.Empty;
        public int LimitedUser { get; set; }
        public bool IsActive { get; set; }
        public int TotalMemberCount { get; set; }
    }
    public class GroupMemModel
    { 
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty; // Leader/Member
        public int Contribution { get; set; }
        public DateTime JoinedAt { get; set; }
    }
    public class GroupDetailListModel : GroupDetailModel
    { 
        public List<GroupModel> Members { get; set; }
        public List<TaskModel> Tasks { get; set; }
        public List<ActivityLogModel> ActivityLogs { get; set; }
        public List<ReportModel> Reports { get; set; }

    }
}
