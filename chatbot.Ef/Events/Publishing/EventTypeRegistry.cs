using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Events.Messages;
using chatbot.Core.Events.Notifications;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Ef.Events.Publishing
{
    public sealed class EventTypeRegistry : IEventTypeRegistry
    {
        private readonly IReadOnlyDictionary<string, Type>  _nameToType;

        private readonly IReadOnlyDictionary<Type, string> _typeToName;

        public EventTypeRegistry()
        {
            var registrations = new Dictionary<string, Type>
            {
                ["chat.message.created.v1"] = typeof(MessageCreatedEvent),

                ["chat.message.edited.v1"] = typeof(MessageEditedEvent),

                ["chat.message.deleted.v1"] = typeof(MessageDeletedEvent),

                ["chat.message.reacted.v1"] = typeof(MessageReactedEvent),

                ["chat.message.recipient-status-changed.v1"] = typeof(MessageRecipientStatusChangedEvent),

                ["chat.message.read.v1"] =typeof(MessagesReadEvent),

                ["chat.conversation.created.v1"] = typeof(ConversationCreatedEvent),

                ["chat.conversation.member-added.v1"] = typeof(MemberAddedEvent),

                ["chat.conversation.member-removed.v1"] = typeof(MemberRemovedEvent),

                ["chat.conversation.member-role-changed.v1"] =typeof(MemberRoleChangedEvent),

                ["chat.notification.created.v1"] = typeof(NotificationCreatedEvent)
            };

            _nameToType = registrations;

            _typeToName =registrations.ToDictionary( x => x.Value,x => x.Key);
        }

        public string GetName(Type eventType)
        {
            if (_typeToName.TryGetValue( eventType,out var name))
            {
                return name;
            }

            throw new InvalidOperationException( $"Event type '{eventType.Name}' is not registered.");
        }

        public string GetName<TEvent>() where TEvent : IEvent
        {
            return GetName(typeof(TEvent));
        }

        public Type GetType(string eventName)
        {
            if (_nameToType.TryGetValue(eventName,out var type))
            {
                return type;
            }

            throw new InvalidOperationException( $"Event name '{eventName}' is not registered.");
        }
    }
}
