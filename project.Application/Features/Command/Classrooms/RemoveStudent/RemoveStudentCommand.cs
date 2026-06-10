using MediatR;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Classrooms.RemoveStudent
{
    public sealed record RemoveStudentCommand(int RequestedBy,  int StudentId, int ClassId) : IRequest<Result>
    {
    }
    internal sealed class RemoveStudentHandler : IRequestHandler<RemoveStudentCommand, Result>
    {
        private readonly IClassroomRepository _classRoomRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveStudentHandler(IClassroomRepository classRoomRepository, IUnitOfWork unitOfWork, IGroupRepository groupRepository)
        {
            _classRoomRepository = classRoomRepository;
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
        }

        public async Task<Result> Handle(RemoveStudentCommand request, CancellationToken cancellationToken)
        {
            var classRoom = await _classRoomRepository.GetClassroomWithEnrollmentsAsync(request.ClassId);
            if (classRoom == null) return Result.Failure(new Error("404", "Không tìm thấy lớp"));

            if (classRoom.TeacherId != request.RequestedBy) return Result.Failure(new Error("403", "Chỉ giáo viên của lớp được thực hiện chức năng này"));

            var student = classRoom.FindEnrollment(request.StudentId);
            if (student == null) return Result.Failure(new Error("404", "Không tìm thấy sinh viên"));

            if (student.GroupId.HasValue)
            {
                var group = await _groupRepository.GetByIdWithMemberAsync(student.GroupId.Value);
                if (group != null)
                {
                    var member = group.FindMember(student.UserId);
                    member?.Leave();
                    await _unitOfWork.Repository<Groups>().UpdateAsync(group);
                }
            }
            classRoom.RemoveStudent(student);
            await _unitOfWork.Repository<Classroom>().UpdateAsync(classRoom);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}
