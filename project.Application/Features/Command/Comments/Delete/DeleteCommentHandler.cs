using MediatR;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;

namespace project.Application.Features.Command.Comments.Delete
{
    public sealed class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IWorkTaskRepository _taskRepository;
        public DeleteCommentHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork, IGroupRepository groupRepository, IWorkTaskRepository taskRepository)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
            _taskRepository = taskRepository;
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(request.CommentId);
                if (comment == null) return Result.Failure(new Error("404", "Không tìm thấy bình luận"));

                var task = await _taskRepository.GetByIdAsync(comment.TaskId);
                if (task == null) return Result.Failure(new Error("404", "Không tìm thấy task"));

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                var member = group.FindMember(request.RequestedBy);
                if (member == null) return Result.Failure(new Error("403", "Bạn không phải thành viên của nhóm"));

                if (!group.IsActive) return Result.Failure(new Error("403", "Không thể xóa bình luận trong nhóm đã vô hiệu hóa"));

                comment.Delete(request.RequestedBy);

                _unitOfWork.Repository<Comment>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch(DomainException ex)
            {
                return Result.Failure(new Error("403",$"{ex.Message}"));
            }
            
        }
    }
}
