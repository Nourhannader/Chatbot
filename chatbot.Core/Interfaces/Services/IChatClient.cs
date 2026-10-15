using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IChatClient
    {
        public interface IChatClient
        {
            // Conversation

            Task ConversationAdded( ConversationDetailsDto conversation);

            Task ConversationUpdated( ConversationDetailsDto conversation);

            Task ConversationDeleted(Guid conversationId);

            Task ConversationImageUpdated(Guid conversationId,ConversationImageDto image);

            Task ConversationImageRemoved(Guid conversationId);


            // Members

            Task MemberAdded(ConversationMemberDto member);

            Task MemberRemoved(Guid userId);

            Task MemberRoleChanged(Guid userId,ConversationRole role);

            Task MemberMuted( Guid userId,DateTime? until);

            Task MemberUnmuted( Guid userId);

            Task MemberBanned(Guid userId);

            Task MemberUnbanned(Guid userId);

            Task OwnershipTransferred( Guid oldOwnerId,Guid newOwnerId);

            Task RemovedFromConversation( Guid conversationId);


            // Messages

            Task ReceiveMessage( MessageDto message);

            Task MessageEdited( MessageDto message);

            Task MessageDeleted(Guid messageId);

            Task MessageReactionAdded( Guid messageId, MessageReactionDto reaction);

            Task MessageReactionRemoved(Guid messageId,Guid userId);


            // Typing

            Task UserTyping( Guid conversationId,Guid userId);

            Task UserStoppedTyping( Guid conversationId, Guid userId);


            // Read

            Task ConversationRead( Guid conversationId,Guid userId,DateTime readAt);
        }
    }
}
