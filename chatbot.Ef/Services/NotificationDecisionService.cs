using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class NotificationDecisionService(IUnitOfWork unitOfWork) : INotificationDecisionService
    {
        public async Task<NotificationOptions> GetOptionsAsync(Guid userId, NotificationType type, Guid? conversationId = null, Guid? messageId = null)
        {
            var saveToDatabase = true;
            var sendSignalR = true;
            var sendPush = true;
            var soundEnabled = true;

            var preferences= await unitOfWork.NotificationPreference.GetByUserIdAsync(userId);

            if(preferences is null)
            {
                sendPush = preferences.PushNotifications;
                soundEnabled = preferences.SoundEnabled;

                var typeEnabled = type switch
                {
                    NotificationType.NewMessage => preferences.NewMessages,

                    NotificationType.MessageReaction => preferences.MessageReactions,

                    NotificationType.MessageReply => preferences.MessageReplies,

                    NotificationType.Mention => preferences.Mentions,

                    _ => true
                };
                if (!typeEnabled)
                {
                    saveToDatabase = false;
                    sendSignalR = false;
                    sendPush = false;
                    soundEnabled = false;
                }
            }

            if (conversationId.HasValue)
            {
                var settings = await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId.Value);
                if (settings is not null)
                {
                    var muted = settings.IsMuted || (settings.MuteUntil.HasValue && settings.MuteUntil.Value > DateTime.UtcNow);

                    if (muted)
                    {
                        sendPush = false;
                        soundEnabled = false;
                    }
                }
            }

            var isOnline= await unitOfWork.UserConnections.HasActiveConnectionAsync(userId);

            if (isOnline)
            {
                sendPush = false;
            }
            return new NotificationOptions
            {
                ConversationId = conversationId,
                MessageId = messageId,

                SaveToDatabase = saveToDatabase,
                SendRealtimeNotification = sendSignalR,
                SendPush = sendPush,
                SoundEnabled = soundEnabled
            };
        }
    }
}
