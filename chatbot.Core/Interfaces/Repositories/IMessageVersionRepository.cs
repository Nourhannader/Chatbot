using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageVersionRepository
    {
            Task AddAsync(MessageVersion version);

            Task<List<MessageVersion>> GetByMessageIdAsync( Guid messageId);
        
    }
}
