using MediatR;
using project.Application.Features.Query.Group.Github;
using project.Application.Interfaces;
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
    public sealed record GetTotalContributionQuery(int RequestBy, int GroupId) : IRequest<Result<IReadOnlyList<MemberContributionModel>>>
    {
    }
    public sealed class GetTotalContributionHandler : IRequestHandler<GetTotalContributionQuery, Result<IReadOnlyList<MemberContributionModel>>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGithubService _githubService;
        public GetTotalContributionHandler(IGroupRepository groupRepository, IGithubService githubService)
        {
            _groupRepository = groupRepository;
            _githubService = githubService;
        }
        public async Task<Result<IReadOnlyList<MemberContributionModel>>> Handle(GetTotalContributionQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("404","Không tìm thấy nhóm"));

            var member = group.FindMember(request.RequestBy);
            if (member == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("403", "Bạn không phải là thành viên của nhóm"));


            if(group.GithubRepoUrl == null) return Result.Failure<IReadOnlyList<MemberContributionModel>>(new Error("400", "Nhóm chưa liên kết với Github"));
            var (owner, repo) = GithubUrlParser.Parse(group.GithubRepoUrl);

            var tasks = group.Members
                            .Where(c => c.IsActive)
                            .Select(async m =>
                            {
                                var contribution = 0;
                                var total = 0.0;
                                if (m.User.GithubUserName != null)
                                {
                                    contribution = await _githubService.GetTotalCommitAsync(owner, repo, m.User.GithubUserName);
                                    total = m.TotalContribution(contribution);
                                }
                                return new MemberContributionModel
                                {
                                    GithubUserName = m.User.GithubUserName,
                                    UserName = m.User.UserName,
                                    TotalContribution = total,
                                    TotalCommit = contribution
                                };
                            });
            var result = await Task.WhenAll(tasks);

            return Result.Success<IReadOnlyList<MemberContributionModel>>(result.OrderByDescending(c => c.TotalContribution).ToList());
        }
    }
}
