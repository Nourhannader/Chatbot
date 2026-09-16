using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Exceptions
{
    public abstract class AppException:Exception
    {
        protected AppException(string message)
        : base(message)
        {
        }
    }
}
