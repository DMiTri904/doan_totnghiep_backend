using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;

namespace project.Application.Features.Command.Group.Reactive
{
    public sealed class ReactiveGroupHandler : IRequestHandler<ReactiveGroupCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReactiveGroupHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(ReactiveGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            if (group == null) return Result.Failure(new Error("404", "Không tìm thấy nhóm"));

            var leader = group.FindMember(request.UserId);
            if (leader == null || !leader.IsLeader()) return Result.Failure(new Error("403", "Chỉ có nhóm trưởng được mở nhóm"));

            group.ReactiveGroup();
            _unitOfWork.Repository<Groups>();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
