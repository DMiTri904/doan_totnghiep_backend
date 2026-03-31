using AutoMapper;
using MediatR;
using project.Application.ModelsDto.DomainModelsDto;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Users
{
    public sealed record GetUsersQuery(string? UserRole, int RequestedBy) : IRequest<Result<IReadOnlyList<UserModel>>>
    {
    }
    public sealed class GetAllUserAppHandler : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserModel>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetAllUserAppHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<UserModel>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.RequestedBy);
            if (user == null || user.UserRole.ToString() != "Admin") return Result.Failure<IReadOnlyList<UserModel>>(new Error("403", "Chỉ có admin được xem tất cả người dùng"));

            UserRole? parsedRole = null;
            if (!string.IsNullOrEmpty(request.UserRole))
            {
                if (!Enum.TryParse<UserRole>(request.UserRole, true, out var parsed))
                    return Result.Failure<IReadOnlyList<UserModel>>(new Error("400", "Role không hợp lệ"));
                parsedRole = parsed;
            }

            var users = await _userRepository.GetAllAsync(parsedRole);
            var mapped = _mapper.Map<IReadOnlyList<UserModel>>(users);
            return Result.Success(mapped);
        }
    }
}
