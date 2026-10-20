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
    public class UserDeviceController(IUserDeviceService userDeviceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var userId = User.GetUserId();

            var devices = await userDeviceService.GetUserDevicesAsync(userId);

            return Ok(ApiResponse<List<UserDeviceDto>>.Ok(devices));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDeviceDto dto)
        {
            var userId = User.GetUserId();

            await userDeviceService.RegisterAsync(userId, dto);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] RemoveDeviceDto dto)
        {
            var userId = User.GetUserId();

            await userDeviceService.RemoveAsync(userId, dto.PushToken, dto.Provider);

            return NoContent();
        }

    }
}
