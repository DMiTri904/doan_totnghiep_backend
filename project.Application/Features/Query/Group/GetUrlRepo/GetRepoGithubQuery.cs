using MediatR;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Group.GetUrlRepo
{
    public sealed record GetRepoGithubQuery(int RequestedBy, int GroupId) : IRequest<Result<string>>
    {
    }
    public sealed class GetRepoGithubHandler : IRequestHandler<GetRepoGithubQuery, Result<string>>
    {
        private readonly IGroupRepository _groupRepository;

        public GetRepoGithubHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<Result<string>> Handle(GetRepoGithubQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure<string>(new Error("404", "Không tìm thấy nhóm"));

            var member = group.FindMember(request.RequestedBy);
            if (member == null) return Result.Failure<string>(new Error("403", "Bạn không phải là thành viên của nhóm"));

            var repoUrl = group.GithubRepoUrl;
            if (repoUrl == null) return Result.Failure<string>(new Error("404", "Nhóm chưa liên kết với repo"));

            return Result.Success(repoUrl);
        }
    }
}
