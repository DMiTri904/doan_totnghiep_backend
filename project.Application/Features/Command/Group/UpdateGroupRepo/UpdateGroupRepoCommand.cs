using MediatR;
using project.Application.Interfaces;
using project.Domain.Exceptions;
using project.Domain.Helpers;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Group.UpdateGroupRepo
{
    public sealed record UpdateGroupRepoCommand(int RequestedBy, int GroupId, string RepoUrl) : IRequest<Result> 
    {
    }
    public sealed class UpdateGroupRepoHandler : IRequestHandler<UpdateGroupRepoCommand, Result>
    {
        private readonly IGithubService _githubService;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateGroupRepoHandler(IGithubService githubService, IGroupRepository groupRepository, IUnitOfWork unitOfWork)
        {
            _githubService = githubService;
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateGroupRepoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader được liên kết đến repo của github"));

                if (!GithubUrlParser.TryParseGithubUrl(request.RepoUrl, out var owner, out var repo))
                    return Result.Failure(new Error("400", "URL repo không hợp lệ"));

                if (!leader.User.LinkedGithubAccount) return Result.Failure(new Error("400", "Tài khoản của bạn chưa liên kết github"));

                var repoOwner = await _githubService.GetRepoOwnerAsync(owner, repo);
                if (!string.Equals(repoOwner, leader.User.GithubUserName, StringComparison.OrdinalIgnoreCase)) return Result.Failure(new Error("403", "Repo này không phải của bạn"));

                group.SetGithubRepoUrl(request.RepoUrl);
                _unitOfWork.Repository<Groups>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch(DomainException ex)
            {
                return Result.Failure(new Error("400",$"{ex.Message}"));
            }
        }

    }
}
