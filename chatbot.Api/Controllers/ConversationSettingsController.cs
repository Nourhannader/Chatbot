using chatbot.Core.Common;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [ApiController]
    [Route("api/conversations/{conversationId:guid}/settings")]
    [Authorize]
    public class ConversationSettingsController(IConversationSettingsService settingService) : ControllerBase
    {
        [HttpPatch("archive")]
        public async Task<IActionResult> Archive(Guid conversationId)
        {
            var userId = User.GetUserId();

            await settingService.ArchiveAsync( User,conversationId);

            return NoContent();
        }

        [HttpPatch("unarchive")]
        public async Task<IActionResult> Unarchive(Guid conversationId)
        {
            var userId = User.GetUserId();

            await settingService.UnarchiveAsync( User,conversationId);

            return NoContent();
        }

        [HttpPatch("pin")]
        public async Task<IActionResult> Pin(Guid conversationId)
        {
            var userId = User.GetUserId();

            await settingService.PinAsync(User, conversationId);

            return NoContent();
        }

        [HttpPatch("unpin")]
        public async Task<IActionResult> UnPin(Guid conversationId)
        {
            var userId = User.GetUserId();

            await settingService.UnPinAsync(User, conversationId);

            return NoContent();
        }

        [HttpPatch("mute")]
        public async Task<IActionResult> Mute(Guid conversationId,[FromBody] MuteConversationDto dto)
        {
            var userId = User.GetUserId();

            await settingService.MuteAsync(User,conversationId, dto.Duration);

            return NoContent();
        }

        [HttpPatch("unmute")]
        public async Task<IActionResult> Unmute(Guid conversationId)
        {
            var userId = User.GetUserId();

            await settingService.UnmuteAsync(User,conversationId);

            return NoContent();
        }

        [HttpGet("mute")]
        public async Task<IActionResult> IsMuted(Guid conversationId)
        {
            var userId = User.GetUserId();

            var isMuted =await settingService.IsMutedAsync(User,conversationId);

            return Ok(ApiResponse<object>.Ok( new
            {
                conversationId,
                isMuted
            }));
        }

    }
}
