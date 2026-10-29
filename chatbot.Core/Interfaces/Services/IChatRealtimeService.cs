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

            Task MessageDeletedAsync(Guid conversationId,string title,DeleteMessageResultDto dto);

            Task MessageReactionChangedAsync(Guid conversationId,chatbot.Core.DTOs.Reactions.ReactionResultDto dto);

            

            // =========================
            // Read
            // =========================

            Task ConversationReadAsync( Guid conversationId,Guid userId, DateTime readAt);
            Task MessageDeliveredAsync(Guid conversationId, MessageRecipientStatusDto dto);
            Task MessagesReadAsync(Guid conversationId, MessagesReadEventDto messagesReadEventDto);
            Task NewMessageAsync(Guid conversationId, MessageDto message);


        //======================
        //Media
        //======================
        Task MediaProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);
        Task VoiceNoteProcessedAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken);
        Task MessageDeletedForUserAsync(Guid userId, MessageDeletedForUserDto dto);
    }
}

