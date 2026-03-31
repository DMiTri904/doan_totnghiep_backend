using MediatR;
using project.Application.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.AISupport
{
    public sealed record EstimateTimeQuery(string Title) : IRequest<Result<string>>
    {
    }
    public sealed class EstimateTimeHandler : IRequestHandler<EstimateTimeQuery, Result<string>>
    {
        private readonly IGeminiService _geminiService;
        public EstimateTimeHandler(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        public async Task<Result<string>> Handle(EstimateTimeQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Result.Failure<string>(new Error("400", "Tiêu đề task không được để trống"));
            var estimate = await _geminiService.EstimateTimeAsync(request.Title);
            if (estimate == null)
                return Result.Failure<string>(new Error("500", "Không thể ước lượng thời gian lúc này"));
            return Result.Success(estimate);
        }
    }
}