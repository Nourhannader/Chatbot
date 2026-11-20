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
    public class SessionRepository(ApplicationDbContext context) : ISessionRepository
    {
        public async Task<int> DeleteExpiredSessionsAsync(DateTime cutoffDate)
        {
            return await context.Sessions
             .Where(x =>
                 (
                     x.ExpiresAt < cutoffDate ||
                     (x.RevokedAt != null &&
                      x.RevokedAt < cutoffDate)
                 ))
             .ExecuteDeleteAsync();
        }

        public async Task<DeviceSession?> GetActiveSessionAsync(Guid sessionId, Guid userId)
        {
            var now = DateTime.UtcNow;

            return await context.Sessions
            .FirstOrDefaultAsync(x =>
                x.Id == sessionId &&
                x.UserId == userId &&
                x.RevokedAt == null &&
                (!x.ExpiresAt.HasValue || x.ExpiresAt > now));
        }

        public async Task<List<DeviceSession>> GetAllActiveSession()
        {
            var now = DateTime.UtcNow;
            return await context.Sessions
                .Include(x => x.User)
                .Where(x =>
                x.RevokedAt == null &&
                (!x.ExpiresAt.HasValue ||
                 x.ExpiresAt > now))
            .ToListAsync();
        }

        public async Task<List<DeviceSession>> GetAllActiveSession(Guid userId)
        {
            var now = DateTime.UtcNow;
            return await context.Sessions
                .Include(x => x.User)
                .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null &&
                (!x.ExpiresAt.HasValue ||
                 x.ExpiresAt > now))
            .ToListAsync();
        }

        public async Task<List<DeviceSession>> GetAllRevokedSessionOlder(DateTime? cutoff)
        {
            if (cutoff is null)
                return new List<DeviceSession>();

            return await context.Sessions
                .Where(x =>
                    x.RevokedAt != null &&
                    x.RevokedAt < cutoff.Value)
                .ToListAsync();
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
