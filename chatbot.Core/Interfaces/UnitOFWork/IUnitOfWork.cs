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
        IUserDeviceRepository UserDevices { get; }
        INotificationRepository Notifications { get; }
        IUserConnectionRepository UserConnections { get; }
        IMessageStatusRepository MessageStatuses { get; }
        IForwardRepository ForwardMessages { get; }
        ISearchRepository Searches { get; }
       
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

    }
}
