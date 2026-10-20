using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.GetUserId();

            var notifications = await notificationService.GetAsync(userId, pageNumber, pageSize);

            return Ok(ApiResponse<PagedResultDto<NotificationDto>>.Ok(notifications));
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.GetUserId();

            var count = await notificationService.GetUnreadCountAsync(userId);

            return Ok(ApiResponse<object>.Ok( new
            {
                unreadCount = count
            }));
        }

        [HttpPatch("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
        Guid notificationId)
        {
            var userId = User.GetUserId();

            await notificationService.MarkAsReadAsync(userId,notificationId);

            return NoContent();
        }


        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.GetUserId();

            await notificationService.MarkAllAsReadAsync(userId);

            return NoContent();
        }
    }
}
