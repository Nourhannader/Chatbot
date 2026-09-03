using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public  Task DeleteAsync(StoredFile file)
        {
           context.StoredFiles.Remove(file);
            return Task.CompletedTask;

        }

        public Task<StoredFile?> GetByIdAsync(Guid id)
        {
            return context.StoredFiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<StoredFile>> GetByMessageIdAsync(Guid messageId)
        {
            return context.StoredFiles
                .Where(x => x.MessageId ==messageId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

       
        public async Task<List<StoredFile>> GetFilesForCleanupAsync(DateTime olderThan)
        {
            return await context.StoredFiles
                            .Where(f => f.IsDeleted && f.DeletedAt < olderThan
                            && f.DeletedAt !=null && !f.IsPhysicallyDeleted)
                            .ToListAsync();
        }

        public Task UpdateAsync(StoredFile file)
        {
            context.StoredFiles.Update(file);
            return Task.CompletedTask;
        }
    }
}
