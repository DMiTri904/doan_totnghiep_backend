using AutoMapper;
using MediatR;
using project.Application.ModelsDto;
using project.Domain.Interfaces;
using project.Domain.Shared;

namespace project.Application.Features.Query.WorkTask.GetById
{
    public sealed class GetTasksInGroupHandler : IRequestHandler<GetTasksInGroupQuery, Result<IReadOnlyList<TaskModel>>>
    {
        private readonly IWorkTaskRepository _taskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GetTasksInGroupHandler(IMapper mapper, IWorkTaskRepository taskRepository, IGroupRepository groupRepository)
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _groupRepository = groupRepository;
        }

        public async Task<Result<IReadOnlyList<TaskModel>>> Handle(GetTasksInGroupQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetByIdWithMemberAsync(request.GroupId);
            var member = group?.FindMember(request.RequestedBy);
            if (member == null) return Result.Failure<IReadOnlyList<TaskModel>>(new Error("403", "Bạn không phải người trong nhóm"));

            var tasks = await _taskRepository.GetTasksByGroupIdAsync(request.GroupId, request.LabelId, request.taskStatus, request.taskPriority);

            var dto = _mapper.Map<IReadOnlyList<TaskModel>>(tasks);

            return Result.Success(dto);
        }
    }
}
