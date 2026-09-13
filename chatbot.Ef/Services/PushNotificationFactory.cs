using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Services
{
    public class PushNotificationFactory : IPushNotificationFactory
    {
        private readonly FirebaseNotificationService firebase;
        private readonly OneSignalPushNotificationService oneSignal;
        public PushNotificationFactory(FirebaseNotificationService firebase,OneSignalPushNotificationService oneSignal)
        {
            this.firebase = firebase;
            this.oneSignal = oneSignal;
        }
        public IPushNotificationService Get(PushProvider provider)
        {
            return provider switch
            {
                PushProvider.Firebase =>
                    firebase,

                PushProvider.OneSignal =>
                    oneSignal,

                _ => throw new
                    NotSupportedException(
                        $"Provider {provider} is not supported")
            };
        }
    }
}
