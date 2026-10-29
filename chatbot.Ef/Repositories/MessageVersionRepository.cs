using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class MessageVersionRepository(ApplicationDbContext context) : IMessageVersionRepository
    {
        public async Task AddAsync(MessageVersion version)
        {
           await context.AddAsync(version);
        }

        public async Task<List<MessageVersion>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.MessageVersions.
                AsNoTracking()
                .Where(x => x.MessageId == messageId)
                .ToListAsync();
        }
    }
}
