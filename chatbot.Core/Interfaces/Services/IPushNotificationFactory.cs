using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IPushNotificationFactory
    {
        IPushNotificationService Get(PushProvider provider);
    }
}
