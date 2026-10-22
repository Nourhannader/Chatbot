using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class MessageAttachmentRepository(ApplicationDbContext context) : IMessageAttachmentRepository
    {
        public async Task AddAsync(MessageAttachment entity)
        {
            await context.MessageAttachments.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<MessageAttachment> attachments)
        {
            await context.MessageAttachments.AddRangeAsync(attachments);
        }

        public async Task<MessageAttachment?> GetByIdAsync(Guid attachmentId)
        {
            return await context.MessageAttachments
                .FirstOrDefaultAsync(x => x.Id == attachmentId);
        }

        public async Task<List<MessageAttachment>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.MessageAttachments
             .Where(x => x.MessageId == messageId)
            .Include(x => x.StoredFile)
                .ThenInclude(x => x.ThumbnailFile)
            .OrderBy(x => x.Order)
            .ToListAsync();
        }

        public async Task<MessageAttachment?> GetWithFileAsync(Guid attachmentId)
        {
            return await context.MessageAttachments
            .Include(x => x.Message)
            .Include(x => x.StoredFile)
                .ThenInclude(x => x.ThumbnailFile)
            .FirstOrDefaultAsync(
                x => x.Id == attachmentId);
        }

        public async Task<MessageAttachment?> GetForDownloadAsync(Guid messageId, Guid attachmentId)
        {
            return await context.MessageAttachments
                .AsNoTracking()
                .Include(x => x.StoredFile)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == attachmentId &&
                        x.MessageId == messageId &&
                        !x.StoredFile.IsDeleted);
        }

        public void Update(MessageAttachment entity)
        {
            context.MessageAttachments.Update(entity);
        }

        public void Remove( MessageAttachment attachment)
        {
            context.MessageAttachments.Remove(attachment);
        }
    }
}
