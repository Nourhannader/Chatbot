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
        public async Task<ConversationUserSettings> GetOrCreateSettingsAsync(Guid userId, Guid conversationId)
        {
            var settings= await context.Settings
                .FirstOrDefaultAsync(s => s.UserId==userId && s.ConversationId==conversationId);

            if (settings != null)
                return settings;

            settings = new ConversationUserSettings
            {
                UserId = userId,
                ConversationId = conversationId
            };

           await context.Settings.AddAsync(settings);

            return settings;

        }
    }
}
