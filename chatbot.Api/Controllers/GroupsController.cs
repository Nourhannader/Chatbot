using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController(IGroupService groupService) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> Create([FromForm] CreateGroupDto dto)
        {
            var userId = User.GetUserId();

            var Id = await groupService.CreateAsync(userId, dto);

            return CreatedAtAction(nameof(GetById),
                new
                {
                    conversationId =Id
                },
                ApiResponse<ConversationDetailsDto>.Ok(Id,"Group created successfully."));
        }
    }
}
