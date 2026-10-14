using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Manager;

namespace chatbot.Ef.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly ConcurrentDictionary< Guid, ConcurrentDictionary<string, byte>>_connections = new();

        public Task AddConnectionAsync(Guid userId,string connectionId)
        {
            var connections =
                _connections.GetOrAdd( userId,_ => new ConcurrentDictionary< string, byte>());

            connections.TryAdd(connectionId,0);

            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync( Guid userId, string connectionId)
        {
            if (_connections.TryGetValue(userId,out var connections))
            {
                connections.TryRemove( connectionId, out _);

                if (connections.IsEmpty)
                {
                    _connections.TryRemove(userId, out _);
                }
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<string>> GetConnectionsAsync(Guid userId)
        {
            if (!_connections.TryGetValue(userId,out var connections))
            {
                return Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
            }

            IReadOnlyCollection<string> result =connections.Keys.ToList();

            return Task.FromResult(result);
        }
    }
}
