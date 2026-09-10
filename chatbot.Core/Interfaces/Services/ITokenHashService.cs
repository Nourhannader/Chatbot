using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface ITokenHashService
    {
        string Hash(string token);

        bool Verify(string token,string hash);
    }
}
