using MediatR;
using project.Application.Interfaces;
using project.Domain.Interfaces;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Auth.Forgot
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand,Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        public ForgotPasswordHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.email)) return Result.Failure(new Error("404", "Email không được để trống"));
            if (string.IsNullOrEmpty(request.clientUri)) return Result.Failure(new Error("404", "ClientUri không được để trống"));



            
        }
    }
}
