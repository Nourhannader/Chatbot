using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class MessageRepository(ApplicationDbContext context) : IMessageRepository
    {
        public async Task AddAsync(Message entity)
        {
            await context.Messages.AddAsync(entity);
        }

        public async Task<Message> GetByIdAsync(string id)
        {
            return await context.Messages
                .Include(m=>m.Reactions)
                .Include(m=> m.RecipientStatuses)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
        }

        public async Task<PagedResultDto<Message>> GetConversationMessagesAsync(string conversationId, int page, int pageSize)
        {
            var items = await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Reactions)
                .Include(m => m.RecipientStatuses)
                .ToListAsync();
            var PagedResult = new PagedResultDto<Message>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = await context.Messages.CountAsync(m => m.ConversationId == conversationId)
            };
            return PagedResult;
        }

        public async Task<List<Message>> SearchMessagesAsync(string conversationId, string keyword)
        {
            return await context.Messages
                .Where(x =>x.ConversationId == conversationId &&x.Content.Contains(keyword))
              .OrderByDescending(x => x.SentAt)
               .ToListAsync();
        }

        public void Update(Message entity)
        {
            context.Messages.Update(entity);
        }
    }
}
