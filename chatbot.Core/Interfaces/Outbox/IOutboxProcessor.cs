using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Outbox
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync( CancellationToken cancellationToken = default);
    }
}
