using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Enums
{
    public enum FileProcessingStatus
    {
        NotRequired = 1,
        Pending = 2,
        Processing = 3,
        Completed = 4,
        Failed = 5
    }
}
