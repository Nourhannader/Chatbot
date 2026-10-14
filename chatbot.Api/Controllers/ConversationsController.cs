using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController(IConversationService conversationService) : ControllerBase
    {
        [HttpPost("private")]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateConversationDto dto)
        {
            var userId = User.GetUserId();
            var secondUserId=Guid.Parse(dto.UserId);
            var result = await conversationService.CreateConversationAsync(userId, secondUserId);
            return Ok(ApiResponse<ConversationDetailsDto>.Ok(result, "Conversation created successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetMyConversations()
        {
            var userId = User.GetUserId();
            var result=await conversationService.GetUserConversationsAsync(userId);
            return Ok(ApiResponse<List<ConversationDto>>.Ok(result, "Conversations retrieved successfully."));
        }
    }
}
