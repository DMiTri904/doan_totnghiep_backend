using MediatR;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Command.Auth.Forgot;
using project.Application.Features.Command.Auth.Login;
using project.Application.Features.Command.Auth.Reset;
using project.Application.ModelsDto;
using project.Presentation.Models.Auth;

namespace project.Presentation.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiController
    {
        public AuthController(ISender sender) : base(sender)
        {
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var command = new LoginCommand(request.MSSV, request.Password);
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var command = new ForgotPasswordCommand(request.Email, request.ClientUri);
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword, request.PasswordConfirm);
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }
    }
}
