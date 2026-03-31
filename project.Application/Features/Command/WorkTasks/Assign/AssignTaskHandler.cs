using MediatR;
using Microsoft.Extensions.Logging;
using project.Application.Interfaces;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.Assign
{
    public sealed class AssignTaskHandler : IRequestHandler<AssignTaskCommand, Result>
    {
        private readonly IWorkTaskRepository _workTaskRepository;
        private readonly INotificationService _notificationService;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignTaskHandler(IUnitOfWork unitOfWork, IWorkTaskRepository workTaskRepository, IGroupRepository groupRepository, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _workTaskRepository = workTaskRepository;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var task = await _workTaskRepository.GetByIdAsync(request.TaskId);
                if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));   

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);

                var leader = group?.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader được giao task"));

                var member = group?.FindMember(request.UserId);
                if (member == null) return Result.Failure(new Error("404", "Người dùng không phải là thành viên trong nhóm"));

                task.Assign(request.UserId);
                _unitOfWork.Repository<WorkTask>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var notification = Notification.Create(request.UserId, $"Bạn được giao task: {task.Title} bởi {leader.User.UserName} trong nhóm {group!.Name}", null, task.GroupId, "Task", task.Id);
                await _notificationService.SendNotificationAsync(notification,cancellationToken);

                return Result.Success();
            }
            catch(DomainException ex)
            {
                return Result.Failure(new Error("401", $"{ex.Message}"));
            }
        }
    }
}
