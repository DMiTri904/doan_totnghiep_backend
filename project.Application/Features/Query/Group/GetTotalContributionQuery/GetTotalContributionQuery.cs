using MediatR;
using project.Application.Features.Query.Group.Github;
using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Domain.Helpers;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Group.GetTotalContributionQuery
{
    public sealed record GetTotalContributionQuery(int RequestedBy, int GroupId) : IRequest<Result<IReadOnlyList<MemberContributionModel>>>
    {
    }
    public sealed class GetTotalContributionHandler : IRequestHandler<GetTotalContributionQuery, Result<IReadOnlyList<MemberContributionModel>>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGithubService _githubService;
        private readonly IClassroomRepository _classRoomRepository;
        public GetTotalContributionHandler(IGroupRepository groupRepository, IGithubService githubService, IClassroomRepository classRoomRepository)
        {
            _groupRepository = groupRepository;
            _githubService = githubService;
            _classRoomRepository = classRoomRepository;
        }
        public async Task<Result<IReadOnlyList<MemberContributionModel>>> Handle(GetTotalContributionQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("404","Không tìm thấy nhóm"));
            if (!group.IsActive) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("403", "Nhóm đã bị vô hiệu hóa"));

            var classRoom = await _classRoomRepository.GetByIdAsync(group.ClassRoomId);
            if (classRoom == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("404", "Không tìm thấy lớp học"));

            if (classRoom.TeacherId != request.RequestedBy)
            {
                var member = group.FindMember(request.RequestedBy);
                if (member == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("403", "Bạn không phải là thành viên của nhóm"));
                if (!member.IsActive) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("403", "Bạn đã rời nhóm"));
            }

            if (group.MajorType == Domain.Models.MajorType.IT) // Caculate IT Students
            {
                if (group.GithubRepoUrl == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("403", "Nhóm chưa liên kết đến repo"));
                var (owner, repo) = GithubUrlParser.Parse(group.GithubRepoUrl);
                var tasks = group.Members
                            .Where(c => c.IsActive)
                            .Select(async m =>
                            {
                                var commit = 0;
                                var total = 0.0;
                                if (m.User.GithubUserName != null)
                                {
                                    commit = await _githubService.GetTotalCommitAsync(owner, repo, m.User.GithubUserName);
                                    total = m.CalculateTotalContributionITStudent(commit);
                                }
                                return new MemberContributionModel
                                {
                                    GithubUserName = m.User.GithubUserName,
                                    UserName = m.User.UserName,
                                    TotalCommit = commit,
                                    TotalContribution = total,
                                };
                            });
                var result = await Task.WhenAll(tasks);
                return Result.Success<IReadOnlyList<MemberContributionModel>>(result.OrderByDescending(c => c.TotalContribution).ToList());
            }
            else // Caculate General Students
            {
                var total = group.Members
                            .Where(c => c.IsActive)
                            .Select(m =>
                            {
                                var total = 0.0;
                                total = m.CalculateTotalContributionGeneralStudent();

                                return new MemberContributionModel
                                {
                                    GithubUserName = m.User.GithubUserName,
                                    UserName = m.User.UserName,
                                    TotalContribution = total,
                                };
                            });
                return Result.Success<IReadOnlyList<MemberContributionModel>>(total.OrderByDescending(c => c.TotalContribution).ToList());
            }
        }
    }
}
