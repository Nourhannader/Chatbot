using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;

namespace chatbot.Core.Interfaces.UnitOFWork
{
    public interface IUnitOfWork : IDisposable
    {
        IMessageRepository Messages { get; }
        IConversationRepository Conversations { get; }
        IReactionRepository Reactions { get; }
        IBlockRepository Blocks { get; }
        Task<int> SaveChangesAsync();

    }
}
