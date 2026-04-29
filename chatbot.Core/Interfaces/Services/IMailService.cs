using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string mailTo, string subject, string body);
    }
}
