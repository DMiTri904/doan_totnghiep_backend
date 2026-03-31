using MediatR;
using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Domain.Helpers;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.WorkTask.GetTaskDetail
{
    public sealed record GetTaskDetailQuery(int TaskId, int RequestedBy) : IRequest<Result<TaskDetailModel>>
    {
    }
    public sealed record TaskDetailModel(int Id, string Title, string? Description, string Status, PullRequestModel? PR = null, string? Assignee = null);

    public sealed class GetTaskDetailHander : IRequestHandler<GetTaskDetailQuery, Result<TaskDetailModel>>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IGithubService _githubService;
        private readonly IGroupRepository _groupRepository;

        public GetTaskDetailHander(IGroupRepository groupRepository, IGithubService githubService, IWorkTaskRepository taskRepository)
        {
            _groupRepository = groupRepository;
            _githubService = githubService;
            _taskRepository = taskRepository;
        }

        public async Task<Result<TaskDetailModel>> Handle(GetTaskDetailQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null) return Result.Failure<TaskDetailModel>(new Error("404", "Không tìm thấy task"));
            
            var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
            if (group == null) return Result.Failure<TaskDetailModel>(new Error("404", "Không tìm thấy nhóm"));

            var member = group.FindMember(request.RequestedBy);
            if (member == null) return Result.Failure<TaskDetailModel>(new Error("403", "Bạn không có quyền xem task này"));

            if (group.GithubRepoUrl == null) return Result.Failure<TaskDetailModel>(new Error("400", "Nhóm không có repo github"));
            var (owner, repo) = GithubUrlParser.Parse(group.GithubRepoUrl!);
            if (task.Assignee == null)
            {
                return new TaskDetailModel(task.Id, task.Title, task.Description, task.Status.ToString());
            }
            PullRequestModel? pr = null;
            if (task.Status == TasksStatus.Test || task.Status == TasksStatus.Done)
            {
                pr = await _githubService.GetPRByTaskIdAsync(owner, repo, task.Id);
            }
            return new TaskDetailModel(task.Id, task.Title, task.Description, task.Status.ToString(), pr, task.Assignee.UserName);
        }
    };

}
