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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.Create
{
    public sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<TaskModel>>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public CreateTaskHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, IMapper mapper, IGroupRepository groupRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
        }

        public async Task<Result<TaskModel>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
                if (group == null) return Result.Failure<TaskModel>(new Error("404", "Không tìm thấy nhóm"));
                
                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure<TaskModel>(new Error("403", "Chỉ có leader được tạo task"));

                if (group.GithubRepoUrl == null) return Result.Failure<TaskModel>(new Error("400", "Nhóm chưa liên kết với Github Repo"));

                var assignee = request.AssignedTo;
                if (assignee.HasValue)
                {
                    var member = group.FindMember(assignee.Value);
                    if (member == null) return Result.Failure<TaskModel>(new Error("404", "Người dùng không phải là thành viên trong nhóm"));
                }

                var task = WorkTask.Create(request.GroupId, request.Title, request.CreatedBy, request.TaskStatus, request.Priority, request.AssignedTo, request.DueDate);
                await _taskRepository.AddAsync(task);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (request.AssignedTo.HasValue)
                {
                    var notification = Notification.Create(request.AssignedTo.Value, $"Bạn có task {task.Title} được giao bởi {task.Creator.UserName} trong nhóm {task.Groups.Name}", null, task.GroupId, "Task", task.Id);
                    await _notificationService.SendNotificationAsync(notification,cancellationToken);
                }

                var dto = _mapper.Map<TaskModel>(task);
                return Result.Success(dto);
            }
            catch( DomainException ex)
            {
                return Result.Failure<TaskModel>(new Error("401", $"{ex.Message}"));
            }
        }
    }
}
