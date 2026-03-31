using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;

namespace project.Application.Features.Command.Group.Deactivate
{
    public sealed class DeactivateGroupHandler : IRequestHandler<DeactivateGroupCommand,Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateGroupHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeactivateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

            var leader = group.FindMember(request.UserId);
            if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có nhóm trưởng được khóa nhóm"));

            group.DeactivateGroup();
            _unitOfWork.Repository<Groups>();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
