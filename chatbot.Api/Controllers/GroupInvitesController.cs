using chatbot.Core.DTOs.InviteGroup;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{

    [ApiController]
    [Route("api/conversations/{conversationId:guid}/invites")]
    [Authorize]
    public class GroupInvitesController(IGroupInviteService inviteService,ICurrentUser currentUser) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<GroupInviteDto>> Create(Guid conversationId, [FromBody] CreateGroupInviteDto dto)
        {
            var result = await inviteService.GenerateAsync( currentUser.Principal,conversationId,dto);

            return Ok(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join(string code)
        {
            await inviteService.JoinAsync(currentUser.Principal,code);

            return NoContent();
        }
        [HttpDelete("{inviteId:guid}")]
        public async Task<IActionResult> Revoke( Guid conversationId, Guid inviteId)
        {
            await inviteService.RevokeAsync(currentUser.Principal, conversationId, inviteId);

            return NoContent();
        }
    }
}
