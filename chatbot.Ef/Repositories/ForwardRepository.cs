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

        public async Task<List<Guid>> GetValidConversationIdsAsync(Guid userId, IEnumerable<Guid> conversationIds)
        {
            var ids = conversationIds.Distinct().ToList();

            return await context.ConversationMembers
                .Where(cm =>
                    ids.Contains(cm.ConversationId) &&
                    cm.UserId == userId &&
                    cm.LeftAt == null &&
                    !cm.IsBanned)
                .Select(cm => cm.ConversationId)
                .Distinct()
                .ToListAsync();
        }
    }
}
