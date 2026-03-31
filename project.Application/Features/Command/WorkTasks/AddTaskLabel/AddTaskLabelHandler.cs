using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.AddTaskLabel
{
    public sealed class AddTaskLabelHandler : IRequestHandler<AddTaskLabelCommand, Result>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddTaskLabelHandler(IWorkTaskRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddTaskLabelCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdWithLabelsAsync(request.TaskId);
            if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

            var label = TaskLabel.Create(request.TaskId,request.LabelId);
            task.AddLabel(label);

            _unitOfWork.Repository<WorkTask>();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
