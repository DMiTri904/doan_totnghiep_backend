using AutoMapper;
using MediatR;
using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.Update
{
    public sealed class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, Result<TaskModel>>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClassroomRepository _classRoomRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public UpdateTaskHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, IMapper mapper, IGroupRepository groupRepository, INotificationService notificationService, IClassroomRepository classRoomRepository)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
            _classRoomRepository = classRoomRepository;
        }
        public async Task<Result<TaskModel>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var task = await _taskRepository.GetByIdAsync(request.Id);
                if (task == null) return Result.Failure<TaskModel>(new Error("404", "Không tìm thấy task"));

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (group == null) return Result.Failure<TaskModel>(new Error("404", "Không có thành viên nào"));
                if (!group.IsActive) return Result.Failure<TaskModel>(new Error("403", "Nhóm đã bị vô hiệu hóa"));

                var classroom = await _classRoomRepository.GetByIdAsync(group.ClassRoomId);
                if (classroom == null) return Result.Failure<TaskModel>(new Error("404", "Không tìm thấy lớp học"));
                if (!classroom.IsActive) return Result.Failure<TaskModel>(new Error("403", "Lớp học đã bị vô hiệu hóa"));

                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure<TaskModel>(new Error("403", "Chỉ có leader được cập nhật task"));

                if (request.AssignedTo.HasValue)
                {
                    var assignee = group.FindMember(request.AssignedTo.Value);
                    if (assignee == null) return Result.Failure<TaskModel>(new Error("404", "Người được giao không phải là thành viên của nhóm"));
                }
                task.UpdateDetails(request.Title, request.Description, request.Priority, request.TaskStatus,request.DueDate, request.AssignedTo);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.AssignedTo.HasValue && request.AssignedTo.Value != leader.UserId)
                {
                    var notification = Notification.Create(request.AssignedTo.Value, $"Bạn đã được giao một task mới trong nhóm {group.Name} bởi {leader.User.UserName}", $"Task: {task.Title}", group.Id, "Task", task.Id);
                    await _notificationService.SendNotificationAsync(notification,cancellationToken);
                }
                var dto = _mapper.Map<TaskModel>(task);

                return Result.Success(dto);
            }
            catch (DomainException ex)
            { 
                return Result.Failure<TaskModel>(new Error("401", $"{ex.Message}"));
            }
            
        }
    }
}
