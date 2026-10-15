using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task<Conversation?> GetByIdAsync(Guid conversationId);

        Task<Conversation?> GetWithMembersAsync(Guid conversationId);

        Task<Conversation?> GetWithImageAsync(Guid conversationId);
    }
}
