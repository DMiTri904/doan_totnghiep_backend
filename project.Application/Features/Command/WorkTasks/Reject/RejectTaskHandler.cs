using MediatR;
using project.Application.Interfaces;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;

namespace project.Application.Features.Command.WorkTasks.Reject
{
    public sealed class RejectTaskHandler : IRequestHandler<RejectTaskCommand, Result>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectTaskHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IWorkTaskRepository taskRepository, IGroupRepository groupRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _taskRepository = taskRepository;
            _groupRepository = groupRepository;
        }

        public async Task<Result> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

            var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
            if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));
            if (!group.IsActive) return Result.Failure(new Error("400", "Nhóm đã bị khóa"));

            var leader = group.FindMember(request.RequestedBy);
            if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Bạn không phải leader để từ chối task này"));

            task.Reject();
            _unitOfWork.Repository<WorkTask>();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var notification = Notification.Create(task.AssignedTo!.Value, task.Title, $"Task '{task.Title}' của bạn bị từ chối bởi {leader.User.UserName} trong nhóm", task.GroupId, "Task", task.Id);
            await _notificationService.SendNotificationAsync(notification, cancellationToken);

            return Result.Success();
        }
    }
}
