using AutoMapper;
using MediatR;
using project.Application.ModelsDto;
using project.Domain.Interfaces;
using project.Domain.Shared;

namespace project.Application.Features.Query.Comment
{
    public sealed class GetCommentsHandler : IRequestHandler<GetCommentsQuery, Result<IReadOnlyList<CommentModel>>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        public GetCommentsHandler(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<CommentModel>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.GetCommentsByTaskIdAsync(request.TaskId);
            if (comments == null) return Result.Success<IReadOnlyList<CommentModel>>(Array.Empty<CommentModel>());

            var dto = _mapper.Map<IReadOnlyList<CommentModel>>(comments);
            return Result.Success(dto);
        }
    }
}
