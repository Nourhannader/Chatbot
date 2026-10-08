using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IReactionRepository :IBaseRepository<MessageReaction,Guid>
    {
        Task<MessageReaction?> GetByMessageAndUserAsync( Guid messageId, Guid userId);

        Task<List<MessageReaction>> GetByMessageIdAsync(Guid messageId);

        void Remove(MessageReaction reaction);
    }
}
