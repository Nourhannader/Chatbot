using System.ComponentModel.Design;
using chatbot.Core.Common;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationPreferenceController(INotificationPreferenceService preferenceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();

            var preferences = await preferenceService.GetAsync(userId);

            return Ok(ApiResponse<NotificationPreferencesDto>.Ok(preferences));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNotificationPreferencesDto dto)
        {
            var userId = User.GetUserId();

            await preferenceService.UpdateAsync(userId, dto);

            return NoContent();
        }

    }
}
