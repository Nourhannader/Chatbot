using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class SearchRepository(ApplicationDbContext context) : ISearchRepository
    {
        public async Task<List<Conversation>> SearchConversationsAsync(Guid userId, string keyword)
        {
            return await context.Conversations
                .Include(x => x.Members)
                .Where(x =>
                x.Title!=null&&
                x.Title.Contains(keyword)&&
                x.Members.Any(m => m.UserId==userId)
                )
                .ToListAsync();
        }

        public async Task<List<Message>> SearchFilesAsync(Guid conversationId, string keyword)
        {
            return await context.Messages
                .Include(x => x.Sender)
                .Where(x => x.ConversationId == conversationId
                ).Include(x => x.StoredFiles.Where(f => f.StoredName.Contains(keyword)))
                .OrderByDescending(x => x.SentAt).ToListAsync();

        }

        public async Task<List<Message>> SearchMessagesAsync(Guid conversationId, string keyword)
        {
            return await context.Messages
                .Include(x => x.Sender)
                .Where(x => x.ConversationId == conversationId &&
                  !x.IsDeletedForEveryone &&
                  x.Content.Contains(keyword)
                ).OrderByDescending(x => x.SentAt)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> SearchUsersAsync(string keyword)
        {
            return await context.Users
               .Where(x => x.FirstName!.Contains(keyword) ||
                x.LastName!.Contains(keyword)||
                x.UserName!.Contains(keyword)
               ).ToListAsync();
        }
    }
}
