using Application.CQRS.Attachment;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AttachmentController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var stream = Request.Body;
            var result = await Mediator.Send(new UploadFileCommand(new AttachmentDto(){ DStream = stream }));
            return result.IsSuccess ? Ok() : BadRequest(result);
        }
    }
}
