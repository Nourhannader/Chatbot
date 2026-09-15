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
    public class SessionRepository(ApplicationDbContext context) : ISessionRepository
    {
        public async Task<List<DeviceSession>> GetAllActiveSession()
        {
            return await context.Sessions
                .Include(x => x.User)
                .Where(x => !x.IsActive
                 && x.ExpiresAt > DateTime.UtcNow
                ).ToListAsync();
        }

        public async Task<List<DeviceSession>> GetAllActiveSession(Guid userId)
        {
            return await context.Sessions
                .Include(x => x.User)
                .Where(x => !x.IsActive
                && x.UserId == userId
                 && x.ExpiresAt > DateTime.UtcNow
                ).ToListAsync();
        }

        public async Task<List<DeviceSession>> GetAllRevokedSessionOlder(DateTime? cutoff)
        {
            return await context.Sessions
                .Where(s => (!s.IsActive && s.RevokedAt < cutoff)
                || s.ExpiresAt < cutoff).ToListAsync();
        }

        public async Task<DeviceSession?> GetById(Guid sessionId)
        {
            return await context.Sessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public  void RemoveRange(List<DeviceSession> sessions)
        {
             context.Sessions.RemoveRange(sessions);
        }
    }
}
