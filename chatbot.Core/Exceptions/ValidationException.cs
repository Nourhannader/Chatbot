using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Exceptions
{
    public class ValidationException:AppException
    {
        public ValidationException(string message):base(message)
        {
            
        }
    }
}
