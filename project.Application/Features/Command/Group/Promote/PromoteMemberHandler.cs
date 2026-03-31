using MediatR;
using project.Application.Interfaces;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Group.Promote
{
    public sealed class PromoteMemberHandler : IRequestHandler<PromoteMemberCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        public PromoteMemberHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        public async Task<Result> Handle(PromoteMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
                if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

                if (!group.IsActive) return Result.Failure(new Error("403", "Không thể thao tác trên nhóm bị vô hiệu hóa"));

                var leader = group.FindMember(request.RequestedBy);
                if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader được quyền thăng chức"));

                var member = group.FindMember(request.UserId);
                if (member == null) return Result.Failure(new Error("404", "Không tìm thấy thành viên"));

                member.PromoteTo(request.Role);
                _unitOfWork.Repository<GroupMem>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var notificication = Notification.Create(member.UserId, $"Bạn được {leader.User.UserName} phân làm {request.Role} trong nhóm {group.Name}", null, group.Id, "Group Member", member.Id);
                await _notificationService.SendNotificationAsync(notificication,cancellationToken);
                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error("401", $"{ex.Message}"));
            }
        }
    }
}
