using chatbot.Api.BackgroundServices;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Events.Media;
using chatbot.Core.Events.Messages;
using chatbot.Core.Events.Notifications;
using chatbot.Core.Interfaces.Events;
using chatbot.Core.Interfaces.Outbox;
using chatbot.Ef.Events.Handlers.Conversations;
using chatbot.Ef.Events.Handlers.Media;
using chatbot.Ef.Events.Handlers.Messages;
using chatbot.Ef.Events.Handlers.Notifications;
using chatbot.Ef.Events.Publishing;
using chatbot.Ef.Outbox;

namespace chatbot.Api.Extensions
{
    public static class EventHandlerExtensions
    {

        public static IServiceCollection AddEventHandlers(
            this IServiceCollection services)
        {
            // =========================================
            // Outbox Infrastructure
            // =========================================

            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<IOutboxProcessor, OutboxProcessor>();

            services.AddSingleton<IEventTypeRegistry, EventTypeRegistry>();
            services.AddSingleton<OutboxMessageFactory>();

            // =========================================
            // Message Created
            // =========================================

            services.AddScoped< IEventHandler<MessageCreatedEvent>,MessageCreatedSignalRHandler>();

            services.AddScoped< IEventHandler<MessageCreatedEvent>,MessageCreatedNotificationHandler>();

            services.AddScoped< IEventHandler<MessageCreatedEvent>, MessageCreatedSearchHandler>();

            // =========================================
            // Message Edited
            // =========================================

            services.AddScoped< IEventHandler<MessageEditedEvent>,MessageEditedSignalRHandler>();

            services.AddScoped< IEventHandler<MessageEditedEvent>, MessageEditedSearchHandler>();

            // =========================================
            // Message Deleted
            // =========================================

            services.AddScoped< IEventHandler<MessageDeletedEvent>, MessageDeletedSignalRHandler>();

            services.AddScoped< IEventHandler<MessageDeletedEvent>, MessageDeletedSearchHandler>();

            // =========================================
            // Message Reacted
            // =========================================

            services.AddScoped< IEventHandler<MessageReactedEvent>, MessageReactedSignalRHandler>();

            services.AddScoped<IEventHandler<MessageReactedEvent>,MessageReactedNotificationHandler>();

            // =========================================
            // Message Status
            // =========================================

            services.AddScoped<IEventHandler<MessageRecipientStatusChangedEvent>, MessageRecipientStatusChangedSignalRHandler>();

            services.AddScoped<IEventHandler<MessagesReadEvent>, MessagesReadSignalRHandler>();

            // =========================================
            // Conversation
            // =========================================

            services.AddScoped<IEventHandler<ConversationCreatedEvent>, ConversationCreatedSignalRHandler>();

            services.AddScoped<IEventHandler<ConversationReadEvent>, ConversationReadSignalRHandler>();

            services.AddScoped< IEventHandler<ConversationUpdatedEvent>, ConversationUpdatedSignalRHandler>();

            services.AddScoped< IEventHandler<ConversationDeletedEvent>, ConversationDeletedSignalRHandler>();

            // =========================================
            // Member Added
            // =========================================

            services.AddScoped< IEventHandler<MemberAddedEvent>, MemberAddedSignalRHandler>();

            services.AddScoped< IEventHandler<MemberAddedEvent>, MemberAddedNotificationHandler>();

            // =========================================
            // Member Removed
            // =========================================

            services.AddScoped<IEventHandler<MemberRemovedEvent>,MemberRemovedSignalRHandler>();

            services.AddScoped< IEventHandler<MemberRemovedEvent>,MemberRemovedNotificationHandler>();

            // =========================================
            // Member Role Changed
            // =========================================

            services.AddScoped<IEventHandler<MemberRoleChangedEvent>, MemberRoleChangedSignalRHandler>();

            services.AddScoped<IEventHandler<MemberRoleChangedEvent>,MemberRoleChangedNotificationHandler>();

            // =========================================
            // Member Joined By Invite
            // =========================================

            services.AddScoped< IEventHandler<MemberJoinedByInviteEvent>,MemberJoinedByInviteSignalRHandler>();

            services.AddScoped< IEventHandler<MemberJoinedByInviteEvent>, MemberJoinedByInviteNotificationHandler>();

            // =========================================
            // Member Unbanned
            // =========================================

            services.AddScoped< IEventHandler<MemberUnbannedEvent>,MemberUnbannedSignalRHandler>();

            services.AddScoped<IEventHandler<MemberUnbannedEvent>, MemberUnbannedNotificationHandler>();

            // =========================================
            // Member Muted
            // =========================================

            services.AddScoped< IEventHandler<MemberMutedEvent>, MemberMutedSignalRHandler>();

            services.AddScoped<IEventHandler<MemberMutedEvent>, MemberMutedNotificationHandler>();

            // =========================================
            // Member Unmuted
            // =========================================

            services.AddScoped< IEventHandler<MemberUnmutedEvent>, MemberUnmutedSignalRHandler>();

            services.AddScoped< IEventHandler<MemberUnmutedEvent>,MemberUnmutedNotificationHandler>();

            // =========================================
            // Notifications
            // =========================================

            services.AddScoped< IEventHandler<NotificationCreatedEvent>, NotificationCreatedSignalRHandler>();

            services.AddScoped< IEventHandler<NotificationCreatedEvent>, NotificationCreatedPushHandler>();

            //=========================================
            // Media Processing
            //=========================================

            services.AddScoped<IEventHandler<MediaProcessedEvent>, MediaProcessedSignalRHandler>();

            services.AddScoped<IEventHandler<VoiceNoteProcessedEvent>,VoiceNoteProcessedSignalRHandler>();

            services.AddScoped< IEventHandler<MediaProcessingFailedEvent>, MediaProcessingFailedSignalRHandler>();

            services.AddScoped<IEventHandler<MediaProcessingRequestedEvent>, MediaProcessingRequestedHandler>();

            services.AddScoped<IEventHandler<VoiceNoteProcessingRequestedEvent>,VoiceNoteProcessingRequestedHandler>();

            // =========================================
            // Background Processing
            // =========================================

            services.AddHostedService<OutboxBackgroundService>();

            return services;
        }

    }
}
