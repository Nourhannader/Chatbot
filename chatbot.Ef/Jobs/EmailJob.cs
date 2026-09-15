using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Jobs
{
    public class EmailJob
    {
        private readonly IMailService mailService;
        public EmailJob(IMailService mailService)
        {
            this.mailService = mailService;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task SendAsync(string email,string subject,string body)
        {
            mailService.SendEmailAsync(email, subject, body);
        }
    }
}
