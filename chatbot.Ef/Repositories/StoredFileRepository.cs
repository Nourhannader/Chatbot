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
        public async Task AddAsync(StoredFile entity)
        {
            await context.StoredFiles.AddAsync(entity);
        }

        public Task<StoredFile?> GetByIdAsync(Guid id)
        {
            return context.StoredFiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<StoredFile>> GetDeletedFilesAsync(DateTime olderThan)
        {
            return await context.StoredFiles
                .Where(f => f.IsDeleted && f.DeletedAt < olderThan)
                .ToListAsync();
        }

        public void Update(StoredFile entity)
        {
            context.StoredFiles.Update(entity);
        }
    }
}
