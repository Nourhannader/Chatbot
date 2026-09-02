using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class ForwardRepository(ApplicationDbContext context) : IForwardRepository
    {
       
        public async Task AddRangeAsync(IEnumerable<Message> messages)
        {
            await context.Messages.AddRangeAsync(messages);
        }

        public async Task<bool> ConversationExistsAsync(Guid conversationId)
        {
            return await context.Conversations
                .AnyAsync(c => c.Id == conversationId);
        }

        public async Task<bool> IsMemberAsync(Guid conversationId, Guid memberId)
        {
            return await context.ConversationMembers
                .AnyAsync(cm => cm.ConversationId == conversationId && cm.UserId==memberId);
        }
    }
}
