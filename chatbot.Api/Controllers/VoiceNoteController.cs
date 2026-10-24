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
    public class VoiceNoteController(IVoiceNoteService voiceNoteService) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<MessageDto>>Send([FromForm] SendVoiceNoteDto dto)
        {
            var userId = User.GetUserId();

            var result = await voiceNoteService.SendAsync(dto,userId);

            return Ok(ApiResponse<MessageDto>.Ok(result, "Send successfully."));
        }
    }
}
