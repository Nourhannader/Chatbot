using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using chatbot.Ef.Repositories;

namespace chatbot.Ef.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        public IMessageRepository Messages { get; private set; }
        public IConversationRepository Conversations { get; private set; }
        public IReactionRepository Reactions { get; private set; }
        public IBlockRepository Blocks { get; private set; }
        public UnitOfWork(ApplicationDbContext _context)
        {
            this.context = _context;
            this.Messages = new MessageRepository(context);
            this.Conversations=new ConversationRepository(context);
            this.Reactions = new ReactionRepository(context);
            this.Blocks = new BlockRepository(context);
        }

        public void Dispose()
        {
          context.Dispose();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();  
        }
    }
}
