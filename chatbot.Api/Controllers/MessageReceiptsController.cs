using chatbot.Core.Common;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageReceiptsController(IMessageStatusService messageStatusService) : ControllerBase
    {
        [HttpGet("{messageId:guid}/receipts")]
        public async Task<IActionResult> GetReceipts(Guid messageId)
        {
            var result = await messageStatusService
                    .GetReceiptsAsync( messageId,User.GetUserId());

            return Ok(ApiResponse<List<MessageRecipientStatusDto>>.Ok(result));
        }

        [HttpGet("{messageId:guid}/status-summary")]
        public async Task<IActionResult> GetSummary( Guid messageId)
        {
            var result = await messageStatusService
                    .GetSummaryAsync(messageId,User.GetUserId());

            return Ok(ApiResponse<MessageStatusSummaryDto>.Ok(result));
        }
    }
}
