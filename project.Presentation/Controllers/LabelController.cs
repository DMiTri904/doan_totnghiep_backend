using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Command.Labels.CreateLabel;
using project.Application.Features.Command.Labels.DeleteLabel;
using project.Application.Features.Command.Labels.UpdateLabel;
using project.Application.Features.Query.Labels.GetLabels;
using project.Presentation.Models;

namespace project.Presentation.Controllers
{
    [Authorize]
    [Route("api/label")]
    public class LabelController : ApiController
    {
        public LabelController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetLabels([FromQuery] int groupId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetLabelsQuery(groupId), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLabel([FromBody] CreateLabelRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateLabelCommand(request.GroupId, request.Name, request.Color);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLabel(int id, [FromBody] UpdateLabelRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateLabelCommand(id, request.Name, request.Color);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(int id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteLabelCommand(id), cancellationToken);
            return result.IsSuccess ? Ok("Xóa label thành công") : BadRequest(result.Error);
        }
    }
}
