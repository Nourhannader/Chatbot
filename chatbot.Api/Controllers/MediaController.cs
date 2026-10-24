using chatbot.Core.DTOs.Messages;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using chatbot.Core.Common;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaController(IMediaMessageService mediaMessageService) : ControllerBase
    {

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<MessageDto>>SendMedia([FromForm] SendMediaDto dto )
        {
            var userId =User.GetUserId();

            var result = await mediaMessageService.SendMediaAsync(dto,userId);

            return Ok(ApiResponse<MessageDto>.Ok(result,"Send successfully."));
        }


        [HttpGet("{messageId:guid}/attachments/{attachmentId:guid}")]
        public async Task<IActionResult> DownloadAttachment(Guid messageId,Guid attachmentId)
        {
            var userId =User.GetUserId();

            var result =await mediaMessageService.GetAttachmentAsync(messageId, attachmentId,userId);

            return File(
                result.Stream,
                result.ContentType,
                result.FileName,
                enableRangeProcessing: true);
        }
    }
}
