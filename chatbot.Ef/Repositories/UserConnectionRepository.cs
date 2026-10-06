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
    public class UserConnectionRepository(ApplicationDbContext context) : IUserConnectionRepository
    {
        public async Task AddAsync(UserConnection entity)
        {
            await context.UserConnections.AddAsync(entity);
        }

        public void Delete(UserConnection connection)
        {
            context.UserConnections.Remove(connection); ;
        }

        public Task<int> GetActiveConnectionCountAsync(Guid userId)
        {
            return context.UserConnections.CountAsync(x => x.UserId == userId && x.IsOnline);
        }

        public async Task<IEnumerable<UserConnection>> GetActiveConnectionsAsync(Guid userId)
        {
            return await context.UserConnections.Where(x => x.UserId == userId && x.IsOnline)
                .OrderByDescending(x => x.ConnectedAt)
                .ToListAsync();
        }

        public async Task<UserConnection?> GetByConnectionIdAsync(string connectionId)
        {
            return await context.UserConnections.FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
        }

        public async Task<UserConnection?> GetByIdAsync(Guid id)
        {
            return await context.UserConnections.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<UserConnection>> GetByUserIdAsync(Guid userId)
        {
            return await context.UserConnections.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ConnectedAt)
                .ToListAsync();
        }

        public async Task<bool> HasActiveConnectionAsync(Guid userId)
        {
            return await context.UserConnections.AnyAsync(x => x.UserId == userId && x.IsOnline);
        }

        public async Task RemoveAsync(string connectionId)
        {
            var connection = await GetByConnectionIdAsync(connectionId);

            if (connection == null)
                return;

            context.UserConnections.Remove(connection);
        }

        public void Update(UserConnection entity)
        {
            context.UserConnections.Update(entity);
        }
    }
}
