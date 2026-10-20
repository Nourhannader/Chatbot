using chatbot.Core.Common;
using chatbot.Core.DTOs.User;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PresenceController(IPresenceService presenceService) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetPresence(Guid userId)
        {
            var presence = await presenceService.GetPresenceAsync(userId);

            return Ok(ApiResponse<UserPresenceDto>.Ok(presence));
        }
    }
}
