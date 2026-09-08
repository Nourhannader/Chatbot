using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace chatbot.Ef.Repositories
{
    public class SearchRepository(ApplicationDbContext context) : ISearchRepository
    {
        public async Task<(List<Conversation> Items, int TotalCount)> SearchConversationsAsync(Guid userId, string keyword, int pageNumber, int pageSize)
        {
            var query = context.Conversations
                .AsNoTracking()
                .Where(c =>
                c.Members.Any(m => m.UserId == userId));

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c => c.Title.Contains(keyword));

            var TotalCount =await query.CountAsync();

            var Items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (Items, TotalCount);
        }

        public async Task<(List<StoredFile> Items, int TotalCount)> SearchFilesAsync(Guid userId, string keyword, int pageNumber, int pageSize)
        {
            var query = context.StoredFiles
                .AsNoTracking()
                .Where(f => f.Message != null &&
                 f.Message.Conversation.Members.All(u => u.UserId == userId)
                );
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(f => f.OriginalName.Contains(keyword));

            var TotalCount = await query.CountAsync();

            var Items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (Items, TotalCount);
        }

        public async Task<(List<Message> Items, int TotalCount)> SearchMessagesAsync(Guid UserId, string keyword, int pageNumber, int pageSize, MessageType? messageType, DateTime? from, DateTime? to)
        {
            var query = context.Messages
                .AsNoTracking()
                .Where(m =>
                m.Conversation.Members.Any(m => m.UserId == UserId));

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(m => m.Content.Contains(keyword));
            if (messageType.HasValue)
                query = query.Where(m => m.MessageType == messageType);
            if (from.HasValue)
                query = query.Where(m => m.SendAt >= from.Value);
            if (to.HasValue)
                query = query.Where(m => m.SendAt <= to.Value);

            var TotalCount = await query.CountAsync();

            var Items = await query
                .OrderByDescending(c => c.SendAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (Items, TotalCount);


        }

        public async Task<(List<ApplicationUser> Items, int TotalCount)> SearchUsersAsync(string keyword, int pageNumber, int pageSize)
        {
            var query = context.Users
                .AsNoTracking()
                .Where(u => u.UserName != null);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(u => u.UserName.Contains(keyword));
            var TotalCount = await query.CountAsync();
            var Items = await query
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return(Items,TotalCount);
                
        }
    }
}
