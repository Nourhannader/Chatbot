using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Ef.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class BlockController(IBlockService blockService,ICurrentUser currentUser) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetBlockedUsers()
        {
            var result = await blockService.GetBlockedUsersAsync(currentUser.UserId);

            return Ok(result);
        }

        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> Block( Guid userId)
        {
            await blockService.BlockAsync(currentUser.UserId, userId);

            return NoContent();
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Unblock( Guid userId)
        {
            await blockService.UnblockAsync(currentUser.UserId, userId);

            return NoContent();
        }

        [HttpGet("{userId:guid}/status")]
        public async Task<ActionResult<bool>> IsBlocked( Guid userId)
        {
            var result = await blockService.IsBlockedAsync(currentUser.UserId, userId);

            return Ok(result);
        }
    }
}
