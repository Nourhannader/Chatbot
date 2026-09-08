using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class ConversationSettingsService(IUnitOfWork unitOfWork) : IConversationSettingsService
    {
        public async Task ArchiveAsync(Guid userId, Guid conversationId)
        {
            var settings = await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId);
            settings.IsArchived = true;
            settings.ArchivedAt=DateTime.UtcNow;
           await unitOfWork.SaveChangesAsync();
        }

        public async Task MuteAsync(Guid userId, Guid conversationId, DateTime? muteUntil)
        {
            var settings =await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId);
            settings.IsMuted = true;
            settings.MuteUntil = muteUntil;
           await unitOfWork.SaveChangesAsync();
        }

        public async Task UnarchiveAsync(Guid userId, Guid conversationId)
        {
            var settings = await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId);
            settings.IsArchived = false;
            settings.ArchivedAt = null;
           await unitOfWork.SaveChangesAsync();
        }

        public async Task UnmuteAsync(Guid userId, Guid conversationId)
        {
            var settings = await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId);
            settings.IsMuted = false;
            settings.MuteUntil = null;
           await unitOfWork.SaveChangesAsync();
        }
    }
}
