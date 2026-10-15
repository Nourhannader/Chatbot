using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId:guid}/members")]
    [Authorize]
    public sealed class GroupMembersController(IGroupService groupService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddMembers(Guid groupId, [FromBody] AddMembersDto dto)
        {
            await groupService.AddMembersAsync(User, groupId, dto);

            return NoContent();
        }


        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            await groupService.RemoveMemberAsync(User, groupId, userId);

            return NoContent();
        }


        [HttpPost("{userId:guid}/promote")]
        public async Task<IActionResult>
            Promote(
                Guid groupId,
                Guid userId)
        {
            await groupService.PromoteAsync(
                User,
                groupId,
                userId);

            return NoContent();
        }


        [HttpPost("{userId:guid}/demote")]
        public async Task<IActionResult>
            Demote(
                Guid groupId,
                Guid userId)
        {
            await groupService.DemoteAsync(
                User,
                groupId,
                userId);

            return NoContent();
        }


        [HttpPost("{userId:guid}/ban")]
        public async Task<IActionResult>
            Ban(
                Guid groupId,
                Guid userId,
                [FromBody] BanMemberDto dto)
        {
            await groupService.BanMemberAsync(
                User,
                groupId,
                userId,
                dto);

            return NoContent();
        }


        [HttpDelete("{userId:guid}/ban")]
        public async Task<IActionResult>
            Unban(
                Guid groupId,
                Guid userId)
        {
            await groupService.UnBanMemberAsync(
                User,
                groupId,
                userId);

            return NoContent();
        }


        [HttpPost("{userId:guid}/Mute")]
        public async Task<IActionResult>
            Mute(
                Guid groupId,
                Guid userId,
                [FromBody] MuteConversationDto dto)
        {
            await groupService.MuteMemberAsync(
                User,
                groupId,
                userId,
                dto);

            return NoContent();
        }


        [HttpDelete("{userId:guid}/Mute")]
        public async Task<IActionResult>
            UnMute(
                Guid groupId,
                Guid userId)
        {
            await groupService.UnMuteMemberAsync(
                User,
                groupId,
                userId);

            return NoContent();
        }
    }
}
