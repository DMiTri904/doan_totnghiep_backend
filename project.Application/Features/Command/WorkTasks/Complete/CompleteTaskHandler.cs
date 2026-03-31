using MediatR;
using project.Application.Interfaces;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.Complete
{
    public sealed class CompleteTaskHandler : IRequestHandler<CompleteTaskCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        public CompleteTaskHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, IGroupRepository groupRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                var leader = group.FindMember(request.RequestedBy);

                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader được phép hoàn thành công việc"));

                var member = group.FindMember(task.AssignedTo!.Value);
                if (member == null) return Result.Failure(new Error("404", "Không tìm thấy thành viên được giao"));

                member.AddContribution();
                task.Complete();
                _unitOfWork.Repository<WorkTask>();
                _unitOfWork.Repository<GroupMem>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var notification = Notification.Create(task.AssignedTo!.Value, $"Task của bạn '{task.Title}' trong nhóm {group!.Name} đã được {leader.User.UserName} duyệt", null, group.Id, "Task", task.Id);
                await _notificationService.SendNotificationAsync(notification,cancellationToken);

                return Result.Success();
            }
            catch(DomainException ex)
            {
                return Result.Failure(new Error("400", $"{ex.Message}"));
            }
        }
    }
}
