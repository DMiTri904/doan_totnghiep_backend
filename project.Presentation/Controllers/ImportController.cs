using MediatR;
using Microsoft.AspNetCore.Mvc;
using project.Application.Features.Command.Import;

namespace project.Presentation.Controllers
{
    public class ImportController : ApiController
    {
        public ImportController(ISender sender) : base(sender)
        {
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportUsers(IFormFile file, CancellationToken cancellationToken)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn file Excel");
            }
            if (!file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Chỉ chấp nhận file .xlsx");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            var fileBytes = stream.ToArray();

            var command = new ImportUserCommand(fileBytes);
            var result = await _sender.Send(command);

            if (result.Failed > 0)
            {
                return Ok(new { Message = $"Import hoàn tất: {result.Success} thành công, {result.Failed} thất bại, {result.Errors}" });
            }
            return Ok(new { Message = $"Import thành công {result.Success} người dùng" });
        }
    }
}
