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

        public async Task<Message> GetByIdAsync(Guid id)
        {
            return await context.Messages
                .Include(m=> m.Sender)
                .Include(m=>m.Reactions)
                .Include(m=> m.RecipientStatuses)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
        }

        public async Task<PagedResultDto<Message>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize)
        {
            var items = await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SendAt)
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

        public async Task<Message?> GetWithFilesAsync(Guid id)
        {
            return  await context.Messages
                .Include(m=> m.Files)
                .Include(m => m.VoiceNote)
                .FirstOrDefaultAsync(m => m.Id == id);
                
        }

        public void Remove(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<List<Message>> SearchMessagesAsync(Guid conversationId, string keyword)
        {
            return await context.Messages
                .Where(x =>x.ConversationId == conversationId &&x.Content.Contains(keyword))
              .OrderByDescending(x => x.SendAt)
               .ToListAsync();
        }

        public void Update(Message entity)
        {
            context.Messages.Update(entity);
        }
    }
}
