using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Command.User;
using project.Application.Features.Command.User.AvatarProfile;
using project.Application.Features.Query.Profile;
using project.Presentation.Extension;
using project.Presentation.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace project.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ApiController
    {
        public UserController(ISender sender) : base(sender)
        {
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var query = new UserProfileQuery(userId.Value);
            var result = await _sender.Send(query);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            if (file.Length == 0 || file == null) return BadRequest("File không được để trống");

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var fileBytes = stream.ToArray();

            var command = new ChangeAvatarCommand(userId.Value, fileBytes,file.FileName,file.ContentType);
            var result = await _sender.Send(command);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var command = new ChangePasswordCommand(userId.Value, request.OldPassword, request.NewPassword, request.PasswordConfirm);
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}


