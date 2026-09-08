using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IConversationSettingsService
    {
        Task MuteAsync(Guid userId,Guid conversationId,DateTime? muteUntil);

        Task UnmuteAsync(Guid userId,Guid conversationId);

        Task ArchiveAsync(Guid userId,Guid conversationId);

        Task UnarchiveAsync(Guid userId,Guid conversationId);
    }
}
