using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IMessageAttachmentRepository : IBaseRepository<MessageAttachment,Guid>
    {
        Task AddRangeAsync(IEnumerable<MessageAttachment> attachments);

        Task<MessageAttachment?> GetByIdAsync(Guid attachmentId);

        Task<List<MessageAttachment>> GetByMessageIdAsync( Guid messageId);

        Task<MessageAttachment?> GetWithFileAsync(Guid attachmentId);

        Task<MessageAttachment?> GetForDownloadAsync(Guid messageId, Guid attachmentId);
        void Remove(MessageAttachment attachment);
    }
}
