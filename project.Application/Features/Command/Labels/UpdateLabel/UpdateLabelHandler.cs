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

namespace project.Application.Features.Command.Labels.UpdateLabel
{
    public sealed class UpdateLabelHandler : IRequestHandler<UpdateLabelCommand, Result<LabelModel>>
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLabelHandler(ILabelRepository labelRepository, IUnitOfWork unitOfWork)
        {
            _labelRepository = labelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LabelModel>> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var label = await _labelRepository.GetByIdAsync(request.Id);
                if (label == null) return Result.Failure<LabelModel>(new Error("404", "Không tìm thấy label"));

                label.Update(request.Name, request.Color);
                _unitOfWork.Repository<Label>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(new LabelModel { Id = label.Id, Name = label.Name, Color = label.Color });
            }
            catch(DomainException ex)
            {
                return Result.Failure<LabelModel>(new Error("400", $"{ex.Message}"));
            }
        }
    }
}
