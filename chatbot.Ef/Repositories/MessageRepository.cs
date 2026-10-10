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

        public async Task AddDeletionAsync(MessageDeletion message)
        {
            await context.Deletions.AddAsync(message);
        }

        public async Task<List<Message>> GetAllMessageDeletedOlder(DateTime cutoff)
        {
            return await context.Messages
                .Where(x =>
                    x.IsDeletedForEveryone &&
                    x.DeletedForEveryoneAt.HasValue &&
                    x.DeletedForEveryoneAt < cutoff)
                .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(Guid id)
        {
            return await context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Reactions)
                .Include(m => m.RecipientStatuses)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Message?> GetMessageByConversationIdAsync(Guid messageId,Guid conversationId)
        {
            return await context.Messages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == messageId &&
                        x.ConversationId == conversationId);
        } 

        public async Task<List<Message>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize)
        {
           return await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Reactions)
                .Include(m => m.Sender)
                .Include(m => m.Files)
                .Include(m => m.RecipientStatuses)
                .ThenInclude(rs => rs.Recipient)
                .OrderByDescending(m => m.SendAt)
                .ThenByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
        }

        public async Task<Message?> GetWithFilesAsync(Guid id)
        {
            return  await context.Messages
                .Include(m=> m.Files)
                .Include(m => m.VoiceNote)
                .FirstOrDefaultAsync(m => m.Id == id);
                
        }

        public async Task<bool> IsDeletedForUserAsync(Guid messageId, Guid userId)
        {
            return await context.Deletions
                .AnyAsync(m => m.UserId == userId && m.MessageId == messageId);
        }

        public void Remove(Message message)
        {
            context.Messages.Remove(message);
        }

        public void RemoveRange(List<Message> messages)
        {
            context.Messages.RemoveRange(messages);
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

        public async Task<Message?> GetWithDetailsAsync(Guid messageId)
        {
            return await context.Messages
                .Include(m => m.SenderId)
                .Include(m => m.Files)
                .Include(m => m.VoiceNote)
                .Include(m => m.Reactions)
                .Include(m => m.RecipientStatuses)
                .ThenInclude(rs => rs.Recipient)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<int> CountByConversationAsync(Guid conversationId)
        {
            return await context.Messages.CountAsync(m => m.ConversationId == conversationId);
        }
    }
}
