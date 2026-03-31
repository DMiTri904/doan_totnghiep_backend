using MediatR;
using project.Application.Interfaces;
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

namespace project.Application.Features.Command.Group.AddMem
{
    public sealed class AddMemberHandler : IRequestHandler<AddMemberCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        public AddMemberHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, INotificationService notificationService)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(AddMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                if (!group.IsActive) return Result.Failure(new Error("403", "Không thể thao tác trên nhóm bị vô hiệu hóa"));

                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader được quyền thêm thành viên"));

                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null) return Result.Failure(new Error("404", "Không tìm thấy người dùng"));

                var member = GroupMem.Create(request.GroupId, request.UserId);
                group.AddMember(member);

                _unitOfWork.Repository<GroupMem>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var notification = Notification.Create(request.UserId, $"Bạn được thêm vào nhóm {group.Name}", null, request.GroupId, "Group", request.GroupId);
                await _notificationService.SendNotificationAsync(notification,cancellationToken);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error("401", $"{ex.Message}"));
            }

        }
    }
}
