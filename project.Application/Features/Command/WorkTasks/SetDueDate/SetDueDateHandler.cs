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

namespace project.Application.Features.Command.WorkTasks.SetDueDate
{
    public sealed class SetDueDateHandler : IRequestHandler<SetDueDateCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SetDueDateHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork, IGroupRepository groupRepository)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
        }

        public async Task<Result> Handle(SetDueDateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

                var members = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (members == null) return Result.Failure<TaskModel>(new Error("404", "Không có thành viên nào"));

                var leader = members.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure<TaskModel>(new Error("403", "Chỉ có leader được cập nhật task"));

                task.SetDueDate(request.dateTime);
                _unitOfWork.Repository<WorkTask>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch(DomainException ex)
            {
                return Result.Failure(new Error("401", $"{ex.Message}"));
            }
        }
    }
}
