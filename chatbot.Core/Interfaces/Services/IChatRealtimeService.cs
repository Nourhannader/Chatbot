using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IChatRealtimeService
    {

        Task SendToConversationAsync( Guid conversationId, string method,object payload,CancellationToken cancellationToken = default);

        Task SendToUserAsync( Guid userId,string method,object payload, CancellationToken cancellationToken = default);
        

        //======================
        //Media
        //======================
        Task MediaProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);
        Task VoiceNoteProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);
        Task MessageDeletedForUserAsync(Guid userId, MessageDeletedForUserDto dto);
    }
}

