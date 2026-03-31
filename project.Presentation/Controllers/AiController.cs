using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Command.WorkTasks.AISupport;
using project.Presentation.Extension;
using project.Presentation.Models;

namespace project.Presentation.Controllers
{
    [Authorize]
    [Route("api/ai")] 
    public class AiController : ApiController
    {
        public AiController(ISender sender) : base(sender)
        {
        }

        [HttpPost("describe")]
        public async Task<IActionResult> Describe([FromBody] AiRequest request)
        {
            var result = await _sender.Send(new GenerateTaskDescriptionQuery(request.Title));
            return result.IsSuccess ? Ok(new { description = result.Value }) : BadRequest(result.Error);
        }

        [HttpPost("subtasks")]
        public async Task<IActionResult> SubTasks([FromBody] AiRequest request)
        {
            var result = await _sender.Send(new SuggestSubTasksQuery(request.Title));
            return result.IsSuccess ? Ok(new { subtasks = result.Value }) : BadRequest(result.Error);
        }

        [HttpPost("estimate")]
        public async Task<IActionResult> Estimate([FromBody] AiRequest request)
        {
            var result = await _sender.Send(new EstimateTimeQuery(request.Title));
            return result.IsSuccess ? Ok(new { estimate = result.Value }) : BadRequest(result.Error);
        }

        [HttpPost("priority")]
        public async Task<IActionResult> Priority([FromBody] AiPriorityRequest request)
        {
            var result = await _sender.Send(new SuggestPriorityQuery(request.Title,request.Description));
            return result.IsSuccess ? Ok(new { priority = result.Value }) : BadRequest(result.Error);
        }
    }
}
