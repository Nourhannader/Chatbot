using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace chatbot.Ef.Services
{
    public class TypingService(ITypingRepository typing) : ITypingService
    {
        public Task<List<Guid>> GetTypingUsersAsync(Guid conversationId)
        => Task.FromResult(typing.GetTypingUsers(conversationId));

        public  Task<bool> IsTypingAsync(Guid conversationId, Guid userId)
            =>Task.FromResult(typing.IsTyping(conversationId, userId));

        public Task<List<(Guid ConversationId, Guid UserId)>> RemoveConnectionAsync(string connectionId)
        {
            return Task.FromResult(typing.RemoveConnection(connectionId));
        }

        public Task StartTypingAsync(Guid conversationId, Guid userId, string connectionId)
        {
            typing.StartTyping(conversationId, userId, connectionId);

            return Task.CompletedTask;
        }

        public Task<bool> StopTypingAsync(Guid conversationId, Guid userId, string connectionId)
        {
            var stillTyping = typing.StopTyping(conversationId, userId,connectionId);

            return Task.FromResult(stillTyping);
        }
    }
}
