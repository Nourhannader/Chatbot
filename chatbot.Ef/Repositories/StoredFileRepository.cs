using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    public class StoredFileRepository(ApplicationDbContext context) : IStoredFileRepository
    {

        public async Task AddAsync(StoredFile file)
        {
            await context.StoredFiles.AddAsync(file);
        }

        public async Task AddRangeAsync(IEnumerable<StoredFile> files)
        {
           await context.StoredFiles.AddRangeAsync(files);
        }

        public  void Delete(StoredFile file)
        {
           context.StoredFiles.Remove(file);

        }

        public Task<StoredFile?> GetByIdAsync(Guid id)
        {
            return context.StoredFiles.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<StoredFile>> GetFilesForCleanupAsync(DateTime olderThan)
        {
            return await context.StoredFiles
                            .Where(f => f.IsDeleted && f.DeletedAt < olderThan
                            && f.DeletedAt !=null && !f.IsPhysicallyDeleted)
                            .ToListAsync();
        }

        public async Task<StoredFile?> GetForProcessingAsync( Guid fileId)
        {
            return await context.StoredFiles
                .Include(x => x.ThumbnailFile)
                .Include(x => x.MessageAttachments)
                    .ThenInclude(x => x.Message)
                .FirstOrDefaultAsync(
                    x => x.Id == fileId &&
                         !x.IsDeleted);
        }

        public async Task<List<StoredFile>> GetPendingFilesAsync( int take)
        {
            return await context.StoredFiles
                .Where(x =>
                   !x.IsDeleted &&
                    x.ProcessingStatus ==
                        FileProcessingStatus.Pending)
                .OrderBy(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
        public void Update(StoredFile file)
        {
            context.StoredFiles.Update(file);
        }

        public async Task<StoredFile?> GetByStorageKeyAsync(string storageKey)
        {
            return await context.StoredFiles
           .FirstOrDefaultAsync(x =>
               x.StorageKey == storageKey);
        }
    }
}
