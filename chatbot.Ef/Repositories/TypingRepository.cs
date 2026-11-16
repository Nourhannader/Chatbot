using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;

namespace chatbot.Ef.Repositories
{
    public sealed class TypingRepository : ITypingRepository
    {
        private static readonly TimeSpan TypingTimeout = TimeSpan.FromSeconds(5);

        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<
                Guid, ConcurrentDictionary<string, DateTime>>> typing = new();
        public List<Guid> GetTypingUsers(Guid conversationId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return [];

            var result = new List<Guid>();

            foreach (var user in users)
            {
                CleanupExpiredConnections(user.Value);

                if (!user.Value.IsEmpty)
                    result.Add(user.Key);
                else
                    RemoveEmptyEntries(conversationId, user.Key, users, user.Value);
            }

            return result;
        }

        public bool IsTyping(Guid conversationId, Guid userId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return false;

            if (!users.TryGetValue(userId, out var connections))
                return false;

            CleanupExpiredConnections(connections);

            return !connections.IsEmpty;
        }

        public List<(Guid ConversationId, Guid UserId)> RemoveConnection(string connectionId)
        {
            var stoppedTyping = new List<(Guid, Guid)>();

            foreach (var conversation in typing)
            {
                foreach (var user in conversation.Value)
                {
                    var connections = user.Value;

                    connections.TryRemove(connectionId, out _);

                    CleanupExpiredConnections(connections);

                    if (connections.IsEmpty)
                    {
                        stoppedTyping.Add((conversation.Key, user.Key));

                        RemoveEmptyEntries(conversation.Key, user.Key, conversation.Value, connections);
                    }
                }
            }

            return stoppedTyping;
        }

        public bool StartTyping(Guid conversationId, Guid userId, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                return false;

            var users = typing.GetOrAdd(conversationId, _ => new());
            var connections = users.GetOrAdd(userId,_ => new());
            connections[connectionId] = DateTime.UtcNow;

            return true;
        }

        public bool StopTyping(Guid conversationId, Guid userId, string connectionId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return false;

            if (!users.TryGetValue(userId, out var connections))
                return false;

            var removed = connections.TryRemove(connectionId, out _);

            RemoveEmptyEntries(conversationId, userId, users, connections);

            return removed && !connections.IsEmpty;
        }

        private static void CleanupExpiredConnections(ConcurrentDictionary<string, DateTime> connections)
        {
            var now = DateTime.UtcNow;

            foreach (var connection in connections)
            {
                if (now - connection.Value <= TypingTimeout)
                    continue;

                // Remove only if the timestamp hasn't been refreshed
                // by a concurrent StartTyping call.
                ((ICollection<KeyValuePair<string, DateTime>>)connections).Remove(connection);
            }
        }

        private void RemoveEmptyEntries(Guid conversationId, Guid userId,
            ConcurrentDictionary<Guid,ConcurrentDictionary<string, DateTime>> users, ConcurrentDictionary<string, DateTime> connections)
        {
            if (!connections.IsEmpty)
                return;

            users.TryRemove(new KeyValuePair<Guid, ConcurrentDictionary<string, DateTime>>(userId,connections));

            if (users.IsEmpty && typing.TryGetValue(conversationId, out var currentUsers) && ReferenceEquals(currentUsers, users))
            {
                typing.TryRemove(
                    new KeyValuePair<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<string, DateTime>>>( conversationId,users));
            }
        }

    }
}
