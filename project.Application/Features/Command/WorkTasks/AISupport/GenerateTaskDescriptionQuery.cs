using MediatR;
using project.Application.Interfaces;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.WorkTasks.AISupport
{
    public sealed record GenerateTaskDescriptionQuery(string TaskTitle) : IRequest<Result<string>>
    {
    }
    public sealed class GenerateTaskDescriptionHandler : IRequestHandler<GenerateTaskDescriptionQuery, Result<string>>
    {
        private readonly IGeminiService _geminiService;

        public GenerateTaskDescriptionHandler(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public async Task<Result<string>> Handle(GenerateTaskDescriptionQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TaskTitle))
                return Result.Failure<string>(new Error("400", "Tiêu đề task không được để trống"));

            var description = await _geminiService.GenerateTaskDescriptionAsync(request.TaskTitle);
            if (description == null)
                return Result.Failure<string>(new Error("500", "Không thể tạo mô tả lúc này"));

            return Result.Success(description);

        }

    }
}
