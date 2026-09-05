using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Services
{
    public class PresenceService(IUnitOfWork unitOfWork) : IPresenceService
    {
        private readonly ConcurrentDictionary<string,ConcurrentDictionary<string, byte>>users = new();

        public Task<int> GetConnectionCountAsync(Guid userId)
        {
            if(users.TryGetValue(userId.ToString(),out var connections))
            {
                return Task.FromResult(connections.Count);
            }
            return Task.FromResult(0);
        }

        public async Task<bool> IsOnlineAsync(Guid userId)
        {
            return await Task.FromResult(users.ContainsKey(userId.ToString()));
        }

        public  Task UserConnectedAsync(Guid userId, string connectionId)
        {
            var connections=users.GetOrAdd(userId.ToString(), _ => new ConcurrentDictionary<string, byte>());
            connections.TryAdd(connectionId, 0);
            return Task.CompletedTask;

        }

        public  Task UserDisconnectedAsync(Guid userId,string connectionId)
        {
           if(users.TryGetValue(userId.ToString(),out var connections))
           {
                connections.TryRemove(connectionId, out _);
                if (connections.IsEmpty)
                {
                    users.TryRemove(userId.ToString(), out _);
                }
           }

           return Task.CompletedTask;
        }
    }
}
