using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum OutboxStatus
    {
        Pending = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4,
        DeadLetter=5
    }
}
