using AutoMapper;
using MediatR;
using project.Application.ModelsDto;
using project.Domain.Exceptions;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Group.Create
{
    public sealed class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupModel>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateGroupHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GroupModel>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var group = Groups.Create(request.NameGroup, request.SubjectName, request.CreateBy, request.LimitedUser);
                
                await _groupRepository.AddAsync(group);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var leader = GroupMem.Create(group.Id,request.CreateBy, GroupMemberRole.Leader);

                group.AddMember(leader);

                _unitOfWork.Repository<Groups>();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<GroupModel>(group);

                return Result.Success(dto);
            }
            catch(DomainException ex)
            {
                return Result.Failure<GroupModel>(new Error("401", $"{ex.Message}"));
            }
        }
    }
}
