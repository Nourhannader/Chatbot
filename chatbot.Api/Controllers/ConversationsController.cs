using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class ConversationsController(IConversationService conversationService) : ControllerBase
    {
        [HttpPost("private")]
        public async Task<ActionResult<ConversationDetailsDto>> CreatePrivate([FromBody] CreatePrivateConversationDto dto)
        {
            var userId = User.GetUserId();
            var secondUserId=Guid.Parse(dto.UserId);
            var result = await conversationService.CreatePrivateAsync(User, dto);
            return Ok(ApiResponse<ConversationDetailsDto>.Ok(result, "Conversation created successfully."));
        }

        [HttpGet("/GetMyConversations")]
        public async Task<IActionResult> GetMyConversations()
        {
            var result=await conversationService.GetMyConversationsAsync(User);
            return Ok(ApiResponse<List<ConversationDto>>.Ok(result, "Conversations retrieved successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<List<ConversationDto>>> GetMine()
        {
            var result = await conversationService.GetMineAsync(User);

            return Ok(result);
        }

        [HttpGet("{conversationId:guid}")]
        public async Task<ActionResult<ConversationDetailsDto>> GetDetails(Guid conversationId)
        {
            var result = await conversationService.GetDetailsAsync(User, conversationId);

            return Ok(ApiResponse<ConversationDetailsDto>.Ok(result,"conversationDetails retrieved successfully."));
        }

        [HttpPut("{conversationId:guid}/image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<ActionResult<ConversationImageDto>> AddOrUpdateImage(Guid conversationId,[FromForm] UpdateConversationImageDto dto)
        {
            var result = await conversationService.AddOrUpdateImageAsync(User, conversationId, dto);

            return Ok(ApiResponse<ConversationImageDto>.Ok(result, "Conversation image updated successfully."));
        }

        [HttpDelete("{conversationId:guid}/image")]
        public async Task<IActionResult> RemoveImage(Guid conversationId)
        {
            await conversationService.RemoveImageAsync(User, conversationId);

            return NoContent();
        }
    }
}
