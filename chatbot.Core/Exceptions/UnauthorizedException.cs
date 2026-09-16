using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Exceptions
{
    public class UnauthorizedException:AppException
    {
        public UnauthorizedException(string message):base(message) { }
        
    }
}
