using chatbot.Core.DTOs.Messages;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Chat;
using chatbot.Core.DTOs.MessageStatus;

namespace chatbot.Api.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageDto message);
        Task MessageUpdated(MessageDto message);
        Task MessageDeleted(Guid messageId);

        Task ReactionUpdated(MessageReactionDto reaction);
        Task MessageStatusUpdated(MessageStatusDto status);

        Task UserTyping(TypingUserDto typing);
        Task UserStoppedTyping(Guid conversationId, Guid userId);

        Task UserOnline(Guid userId);
        Task UserOffline(Guid userId);

        Task ConversationUpdated(ConversationDto conversation);

        Task MemberAdded(Guid conversationId, UserDto user);
        Task MemberRemoved(Guid conversationId, Guid userId);
        Task MemberRoleChanged(Guid conversationId, Guid userId,string role);

        Task NotificationReceived(NotificationDto notification);

        // Media processing
        Task MediaProcessed(Guid conversationId, Guid messageId);
        Task VoiceNoteProcessed(Guid conversationId, Guid messageId);

        // Per-user message deletion
        Task MessageDeletedForUser(MessageDeletedForUserDto dto);
    }
}
