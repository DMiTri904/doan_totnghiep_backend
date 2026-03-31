using AutoMapper;
using MediatR;
using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Comments.Create
{
    public sealed class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CommentModel>>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateCommentHandler(IWorkTaskRepository taskRepository, IMapper mapper, IUnitOfWork unitOfWork, IGroupRepository groupRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
            _notificationService = notificationService;
        }

        public async Task<Result<CommentModel>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null) return Result.Failure<CommentModel>(new Error("404", "Không tìm thấy task"));

                var group = await _groupRepository.GetByIdWithMemberAsync(task.GroupId);
                if (group == null) return Result.Failure<CommentModel>(new Error("404", "Không tìm thấy nhóm"));
                if (!group.IsActive) return Result.Failure<CommentModel>(new Error("403", "Không thể thêm bình luận vào nhóm bị vô hiệu hóa"));

                var member = group.FindMember(request.UserId);
                if (member == null) return Result.Failure<CommentModel>(new Error("403", "Bạn không phải là thành viên trong nhóm"));

                var comment = Comment.Create(request.TaskId, request.UserId, request.Content, request.ParentCommentId);

                await _unitOfWork.Repository<Comment>().AddAsync(comment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if(comment.UserId != request.UserId)
                {
                    var notification = Notification.Create(task.AssignedTo!.Value, $"{member.User.UserName} vừa bình luận vào task {task.Title} trong nhóm {group.Name}", null, group.Id, "Comment", comment.Id);
                    await _notificationService.SendNotificationAsync(notification,cancellationToken);
                }
                var dto = _mapper.Map<CommentModel>(comment);
                return Result.Success(dto);

            }
            catch (DomainException ex)
            {
                return Result.Failure<CommentModel>(new Error("400", $"{ex.Message}"));
            }
         
        }
    }
}
