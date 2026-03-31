using AutoMapper;
using MediatR;
using project.Application.ModelsDto;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Group.GetDetailGroup
{
    public sealed class GetDetailGroupHandler : IRequestHandler<GetDetailGroupQuery, Result<GroupDetailModel>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        public GetDetailGroupHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<Result<GroupDetailModel>> Handle(GetDetailGroupQuery request, CancellationToken cancellationToken)
        {

            var group = await _groupRepository.GetByIdWithDetailAsync(request.GroupId);
            if (group == null) return Result.Failure<GroupDetailModel>(new Error("404", "Không tìm thấy nhóm"));

            var member = group.FindMember(request.RequestedBy);
            if (member == null) return Result.Failure<GroupDetailModel>(new Error("403", "Bạn không phải là thành viên của nhóm"));

            var dto = _mapper.Map<GroupDetailModel>(group);
            dto.ActiveMemberCount = group.ActiveMemberCount();
            dto.PendingTaskCount = group.PendingTaskCount();
            dto.TotalTaskCount = group.TotalTaskCount();

            return Result.Success(dto);
        }
    }
}
