using MediatR;
using project.Application.ModelsDto;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Labels.GetLabels
{
    public sealed class GetLabelsHandler : IRequestHandler<GetLabelsQuery, Result<List<LabelModel>>>
    {
        private readonly ILabelRepository _labelRepository;

        public GetLabelsHandler(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<Result<List<LabelModel>>> Handle(GetLabelsQuery request, CancellationToken cancellationToken)
        {
            var labels = await _labelRepository.GetAllAsync();
            var dto = labels.Select(l => new LabelModel { Id = l.Id, Name = l.Name, Color = l.Color }).ToList();
            return Result.Success(dto);
        }
    }
}
