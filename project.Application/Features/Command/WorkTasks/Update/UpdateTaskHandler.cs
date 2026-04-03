using AutoMapper;
using MediatR;
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
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public UpdateTaskHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, IMapper mapper, IGroupRepository groupRepository)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _groupRepository = groupRepository;
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

                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure<TaskModel>(new Error("403", "Chỉ có leader được cập nhật task"));

                task.UpdateDetails(request.Title, request.Description, request.Priority, request.TaskStatus,request.DueDate, request.AssignedTo);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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
