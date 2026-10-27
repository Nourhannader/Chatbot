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
        Task MediaProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);
        Task MessageCreatedAsync(Guid conversationId, object value, CancellationToken cancellationToken);
        Task MessageSentAsync(Guid conversationId, MessageDto resultDto, CancellationToken cancellationToken);
        Task SendToConversationAsync(Guid conversationId, string v, object value, CancellationToken cancellationToken);
        Task SendToUserAsync(Guid userId, string v, object payload, CancellationToken cancellationToken);
        Task VoiceNoteProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);

        public interface IChatRealtimeService
        {
            // =========================
            // Conversation
            // =========================

            Task ConversationAddedAsync(Guid userId,ConversationDetailsDto conversation);

            Task ConversationUpdatedAsync(Guid conversationId,ConversationDetailsDto conversation);

            Task ConversationDeletedAsync( Guid conversationId);

            Task ConversationImageUpdatedAsync(Guid conversationId,ConversationImageDto image);

            Task ConversationImageRemovedAsync(Guid conversationId);


            // =========================
            // Members
            // =========================

            Task MemberAddedAsync(Guid conversationId, ConversationMemberDto member);

            Task MemberRemovedAsync( Guid conversationId,Guid userId);

            Task MemberRoleChangedAsync(Guid conversationId, Guid userId,ConversationRole role);

            Task MemberMutedAsync(Guid conversationId,Guid userId,DateTime? until);

            Task MemberUnmutedAsync( Guid conversationId,Guid userId);

            Task MemberBannedAsync(Guid conversationId,Guid userId);

            Task MemberUnbannedAsync( Guid conversationId,Guid userId);

            Task OwnershipTransferredAsync(Guid conversationId,Guid oldOwnerId, Guid newOwnerId);

            Task RemoveUserFromConversationAsync( Guid conversationId,Guid userId);


            // =========================
            // Messages
            // =========================

            Task MessageSentAsync( Guid conversationId,MessageDto message);

            Task MessageEditedAsync(Guid conversationId,MessageDto message);

            Task MessageDeletedAsync(Guid conversationId, Guid messageId);

            Task MessageReactionAddedAsync(Guid conversationId,Guid messageId, MessageReactionDto reaction);

            Task MessageReactionRemovedAsync( Guid conversationId, Guid messageId,Guid userId);


            // =========================
            // Read
            // =========================

            Task ConversationReadAsync( Guid conversationId,Guid userId, DateTime readAt);
            Task MessageDeliveredAsync(Guid conversationId, MessageRecipientStatusDto dto);
            Task MessagesReadAsync(Guid conversationId, MessagesReadEventDto messagesReadEventDto);
            Task NewMessageAsync(Guid conversationId, MessageDto message);

        }
    }
}
