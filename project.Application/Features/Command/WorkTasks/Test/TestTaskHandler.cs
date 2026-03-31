using MediatR;
using project.Application.Interfaces;
using project.Domain.Exceptions;
using project.Domain.Helpers;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;

namespace project.Application.Features.Command.WorkTasks.Test
{
    public sealed record TestTaskHandler : IRequestHandler<TestTaskCommand, Result>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly INotificationService _notificationService;
        private readonly IGithubService _githubService;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TestTaskHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, INotificationService notificationService, IGroupRepository groupRepository, IGithubService githubService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _groupRepository = groupRepository;
            _githubService = githubService;
        }

        public async Task<Result> Handle(TestTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                var leader = group.FindMember(task.CreatedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("404", "Không tìm thấy người tạo task"));

                var member = group.FindMember(request.RequestedBy);
                if (member == null) return Result.Failure(new Error("404", "Bạn không phải người dùng của nhóm"));

                var (owner, repo) = GithubUrlParser.Parse(group.GithubRepoUrl!);
                var branchExists = await _githubService.IsBranchExistAsync(owner, repo, task.Id);

                task.Test();
                _unitOfWork.Repository<WorkTask>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var notification = Notification.Create(
                                                leader.UserId,
                                                $"{member.User.UserName} đã gửi task '{task.Title}' vào mục Test trong nhóm {group.Name}",
                                                $"Branch: feature/task-{task.Id}",
                                                task.GroupId,
                                                "Task",
                                                task.Id);

                await _notificationService.SendNotificationAsync(notification,cancellationToken);
                return Result.Success();

            }catch (DomainException ex)
            {
                return Result.Failure(new Error("400", $"{ex.Message}"));
            }
        }
    }
}
