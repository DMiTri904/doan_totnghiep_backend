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
    public sealed record SuggestPriorityQuery(string TaskTitle, string TaskDescription) : IRequest<Result<string>>
    {
    }
    public sealed class SuggestPriorityHandler : IRequestHandler<SuggestPriorityQuery, Result<string>>
    {
        private readonly IGeminiService _geminiService;
        public SuggestPriorityHandler(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        public async Task<Result<string>> Handle(SuggestPriorityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.TaskTitle))
                    return Result.Failure<string>(new Error("400", "Tiêu đề task không được để trống"));
                var priority = await _geminiService.SuggestPriorityAsync(request.TaskTitle, request.TaskDescription);
                if (priority == null)
                    return Result.Failure<string>(new Error("500", "Không thể gợi ý mức độ ưu tiên lúc này"));
                return Result.Success(priority);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new Error("500", $"Đã xảy ra lỗi khi gợi ý mức độ ưu tiên: {ex.Message}"));
            }
        }
    }
}
