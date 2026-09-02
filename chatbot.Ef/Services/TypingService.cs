using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Repositories;
using chatbot.Ef.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace chatbot.Ef.Services
{
    public class TypingService(TypingRepository typing) : ITypingService
    {
        

        public Task<List<string>> GetTypingUsersAsync(Guid conversationId)
        {
            return Task.FromResult(
           typing.GetTypingUsers(
               conversationId));
        }

        public async Task<bool> IsTypingAsync(Guid conversationId, Guid userId)
        {
            return await Task.FromResult(
                typing.IsTyping(
                    conversationId,
                    userId));
        }

        public Task StartTypingAsync(Guid conversationId, Guid userId)
        {
            typing.StartTyping(conversationId, userId);
            return Task.CompletedTask;
        }

        public Task StopTypingAsync(Guid conversationId, Guid userId)
        {
            typing.StopTyping(conversationId, userId);
            return Task.CompletedTask;
        }

       
    }
}
