using chatbot.Api.Extensions;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageReactionsController(IMessageReactionService reactionService,ICurrentUser currentUser) : ControllerBase
    {
        [HttpGet("{messageId:guid}")]
        public async Task<ActionResult<List<MessageReactionDto>>> GetReactions(Guid messageId)
        {
            var userId = currentUser.UserId;

            var result = await reactionService.GetByMessageIdAsync(userId, messageId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ReactionResultDto>> ToggleReaction([FromBody] AddReactionDto dto)
        {
            var userId = currentUser.UserId;

            var result = await reactionService.ToggleReactionAsync(userId, dto);

            return Ok(result);
        }

    }
}
