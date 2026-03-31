using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Group.Delete
{
    public sealed class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteGroupHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));
            
            var leader = group.FindMember(request.RequetedBy);
            if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có leader mới được xóa nhóm"));

            _unitOfWork.Repository<Groups>().DeleteAsync(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
