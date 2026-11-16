using chatbot.Api.Hubs;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Manager;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace chatbot.Api.Services
{
    public sealed class ChatRealtimeService : IChatRealtimeService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRealtimeService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendToConversationAsync(Guid conversationId, string method,object payload, CancellationToken cancellationToken = default)
        {
            var groupName =ChatHubConstants.GetConversationGroup(conversationId);

            return _hubContext.Clients
                .Group(groupName)
                .SendAsync(method, payload, cancellationToken);
        }

        public Task SendToUserAsync(Guid userId,string method,object payload,CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients
                .User(userId.ToString())
                .SendAsync(method, payload, cancellationToken);
        }

        public Task MediaProcessedAsync(Guid conversationId,Guid messageId,CancellationToken cancellationToken)
        {
            return SendToConversationAsync(
                conversationId,
                nameof(IChatClient.MediaProcessed),
                new
                {
                    ConversationId = conversationId,
                    MessageId = messageId
                },
                cancellationToken);
        }

        public Task VoiceNoteProcessedAsync( Guid conversationId,Guid messageId, CancellationToken cancellationToken)
        {
            return SendToConversationAsync(
                conversationId,
                nameof(IChatClient.VoiceNoteProcessed),
                new
                {
                    ConversationId = conversationId,
                    MessageId = messageId
                },
                cancellationToken);
        }

        public Task MessageDeletedForUserAsync(Guid userId, MessageDeletedForUserDto dto)
        {
            return SendToUserAsync(userId, nameof(IChatClient.MessageDeletedForUser),dto);
        }
    }

}
