using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class EmailJob
    {
        private readonly IMailService mailService;
        private readonly ILogger<EmailJob> logger;  
        public EmailJob(IMailService mailService,ILogger<EmailJob> logger)
        {
            this.mailService = mailService;
            this.logger = logger;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task SendAsync(string email,string subject,string body)
        {
            logger.LogInformation("Email job Started for {email}", email);
            mailService.SendEmailAsync(email, subject, body);
            logger.LogInformation("Email Sent Sucessfully to {email}", email);
        }
    }
}
