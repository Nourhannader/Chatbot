using chatbot.Api.Extensions;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController(IMessageService messageService,IForwardService forwardService,ICurrentUser currentUser) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
        {
            var userId = currentUser.UserId;

            var result = await messageService.SendMessageAsync(userId,dto, cancellationToken);

            return Ok(result);
        }

        [HttpGet("conversation/{conversationId:guid}")]
        public async Task<ActionResult<PagedResultDto<MessageDto>>> GetMessages( Guid conversationId,[FromQuery] int page = 1,[FromQuery] int pageSize = 20)
        {
            var userId = currentUser.UserId;

            var result =await messageService.GetMessagesAsyns(userId,conversationId,page,pageSize);

            return Ok(result);
        }

        [HttpPost("reply")]
        public async Task<ActionResult<MessageDto>> Reply([FromBody] ReplyMessageDto dto)
        {
            var userId = currentUser.UserId;

            var result = await messageService.ReplyAsync(userId, dto);

            return Ok(result);
        }

        [HttpPost("forward")]
        public async Task<ActionResult<List<MessageDto>>> Forward([FromBody] ForwardMessageDto dto)
        {
            var userId = currentUser.UserId;

            var result = await forwardService.ForwardAsync(userId, dto);

            return Ok(ApiResponse<List<MessageDto>>.Ok(result));
        }

        //delete for me
        [HttpDelete("{messageId:guid}/me")]
        public async Task<ActionResult<DeleteForMeResultDto>> DeleteForMe(Guid messageId)
        {
            var userId = currentUser.UserId;

            var result = await messageService.DeleteForMeAsync(messageId, userId);

            return Ok(result);
        }

        //delete for everyone

        [HttpDelete("{messageId:guid}/everyone")]
        public async Task<ActionResult<DeleteMessageResultDto>> DeleteForEveryone(Guid messageId)
        {
            var userId = currentUser.UserId;

            var result = await messageService.DeleteForEveryoneAsync(messageId, userId);

            return Ok(result);
        }

        [HttpPost("file")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<MessageDto>> SendFile([FromForm] SendFileDto dto)
        {
            var userId = currentUser.UserId;

            var result = await messageService.SendFileAsync(userId, dto);

            return Ok(result);
        }
    }
}
