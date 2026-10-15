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
    public sealed class GroupsController(IGroupService groupService) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ConversationDetailsDto>>
         Create(
             [FromForm] CreateGroupDto dto)
        {
            var result =
                await groupService.CreateAsync(User, dto);

            return Ok(result);
        }

        [HttpPatch("{groupId:guid}")]
        public async Task<ActionResult<ConversationDetailsDto>> Update(Guid groupId, [FromBody] UpdateGroupDto dto)
        {
            var result =
                await groupService.UpdateAsync(User, groupId, dto);

            return Ok(result);
        }

        [HttpDelete("{groupId:guid}")]
        public async Task<IActionResult> Delete(Guid groupId)
        {
            await groupService.DeleteAsync(User, groupId);

            return NoContent();
        }

        [HttpPost("{groupId:guid}/leave")]
        public async Task<IActionResult> Leave(Guid groupId)
        {
            await groupService.LeaveAsync(User, groupId);

            return NoContent();
        }
    }
}
