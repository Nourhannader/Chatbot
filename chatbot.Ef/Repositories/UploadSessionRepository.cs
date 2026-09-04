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
    public class UploadSessionRepository(ApplicationDbContext context) : IUploadSessionRepository
    {
        public async Task AddAsync(UploadSession entity)
        {
            await context.UploadSessions.AddAsync(entity);
        }

        public async Task<UploadSession?> GetByIdAsync(Guid id)
        {
            return await context.UploadSessions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<UploadSession>> GetExpiredSessionsAsync(DateTime expiredBefore)
        {
            return context.UploadSessions
                .Where(x => !x.IsCompleted &&
                x.CreatedAt < expiredBefore)
                .ToListAsync();
        }

        public void Remove(UploadSession uploadSession)
        {
            context.UploadSessions.Remove(uploadSession);
        }

        public void Update(UploadSession entity)
        {
            context.UploadSessions.Update(entity);
        }
    }
}
