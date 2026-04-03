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
    public sealed record SuggestSubTasksQuery(string Content) : IRequest<Result<string>>
    {
    }
    public sealed class SuggestSubTasksHandler : IRequestHandler<SuggestSubTasksQuery, Result<string>>
    {
        private readonly IGeminiService _geminiService;
        public SuggestSubTasksHandler(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        public async Task<Result<string>> Handle(SuggestSubTasksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content))
                {
                    return Result.Failure<string>(new Error("404", "Nội dung không được để trống"));
                }
                var result = await _geminiService.SuggestSubTasksAsync(request.Content);
                if (string.IsNullOrEmpty(result))
                {
                    return Result.Failure<string>(new Error("500", "Đã có lỗi xảy ra khi gọi Gemini"));
                }
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new Error("500", $"Đã có lỗi xảy ra: {ex.Message}"));
            }
        }
    }
}