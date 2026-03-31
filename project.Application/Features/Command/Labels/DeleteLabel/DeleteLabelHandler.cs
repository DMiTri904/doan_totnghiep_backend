using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Labels.DeleteLabel
{
    public sealed class DeleteLabelHandler : IRequestHandler<DeleteLabelCommand, Result>
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLabelHandler(ILabelRepository labelRepository, IUnitOfWork unitOfWork)
        {
            _labelRepository = labelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
        {
            var label = await _labelRepository.GetByIdAsync(request.Id);
            if (label == null) return Result.Failure(new Error("404", "Không tìm thấy label"));

            if (label.IsUsed()) return Result.Failure(new Error("400", "Label đang được sử dụng, không thể xóa"));

            _labelRepository.DeleteAsync(label);
            _unitOfWork.Repository<Label>();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
