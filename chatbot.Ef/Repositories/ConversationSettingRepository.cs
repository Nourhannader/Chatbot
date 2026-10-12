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
    public class ConversationSettingRepository(ApplicationDbContext context) : IConversationSettingRepository
    {
        public Task AddAsync(ConversationUserSettings settings)
        {
            return context.Settings.AddAsync(settings).AsTask();
        }

        public Task<ConversationUserSettings?> GetAsync(Guid userId, Guid conversationId)
        {
            return context.Settings.FirstOrDefaultAsync(x => x.UserId == userId && x.ConversationId == conversationId);
        }

        public async Task<ConversationUserSettings> GetOrCreateSettingsAsync(Guid userId, Guid conversationId)
        {
            var settings = await GetAsync(userId, conversationId);

            if (settings != null)
                return settings;

            settings = new ConversationUserSettings
            {
                Id=Guid.NewGuid(),
                UserId = userId,
                ConversationId = conversationId
            };

           await context.Settings.AddAsync(settings);

            return settings;

        }
    }
}
