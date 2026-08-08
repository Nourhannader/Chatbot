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

        public async Task<UserConnection?> GetByIdAsync(string id)
        {
            return await context.UserConnections
                .FirstOrDefaultAsync(uc => uc.ConnectionId == id);
        }

        public async Task<List<UserConnection>> GetUserConnectionsAsync(string userId)
        {
            return await context.UserConnections
                .Where(uc => uc.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> HasConnectionsAsync(string userId)
        {
            return await context.UserConnections
                .AnyAsync(us => us.UserId == userId);
        }

        public async Task RemoveAsync(string connectionId)
        {
            var connection =await context.UserConnections
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);
            if(connection != null)
            {
                context.UserConnections.Remove(connection);
            }
        }

        public void Update(UserConnection entity)
        {
            context.UserConnections.Update(entity);
        }
    }
}
