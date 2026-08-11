using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using chatbot.Ef.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace chatbot.Ef.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction? transaction;
        public IMessageRepository Messages { get; private set; }
        public IConversationRepository Conversations { get; private set; }
        public IReactionRepository Reactions { get; private set; }
        public IBlockRepository Blocks { get; private set; }
        public IUserDeviceRepository UserDevices { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IUserConnectionRepository UserConnections { get; private set; }
        public IMessageStatusRepository MessageStatuses { get; private set; }
        public IForwardRepository ForwardMessages {  get; private set; }
        public ISearchRepository Searches { get; private set; }
        public UnitOfWork(ApplicationDbContext _context)
        {
            this.context = _context;
            this.Messages = new MessageRepository(context);
            this.Conversations=new ConversationRepository(context);
            this.Reactions = new ReactionRepository(context);
            this.Blocks = new BlockRepository(context);
            this.UserDevices = new UserDeviceRepository(context);
            this.Notifications = new NotificationRepository(context);
            this.UserConnections = new UserConnectionRepository(context);
            this.MessageStatuses = new MessageStatusRepository(context);
            this.ForwardMessages = new ForwardRepository(context);
            this.Searches = new SearchRepository(context);
        }

        public void Dispose()
        {
            transaction?.Dispose();
            context.Dispose();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();  
        }

        public async Task BeginTransactionAsync()
        {
            transaction =await context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (transaction ==null)
            {
                return;
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if(transaction == null)
            {
                return;
            }
            await transaction.RollbackAsync();
        }
    }
}
