using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageStatusRepository:IBaseRepository<MessageRecipientStatus,string>
    {
        Task<MessageRecipientStatus?> GetAsync(string messageId, string recipientId);
        Task<List<MessageRecipientStatus>> GetByMessageAsync(string messageId);

    }
}
