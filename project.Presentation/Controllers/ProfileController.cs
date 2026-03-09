using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Query.Profile;
using project.Presentation.Extension;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace project.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ApiController
    {
        public ProfileController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            var query = new UserProfileQuery(userId.Value);
            var result = await _sender.Send(query);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
