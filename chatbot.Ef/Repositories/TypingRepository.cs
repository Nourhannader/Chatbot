using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Ef.Repositories
{
    public class TypingRepository
    {
        private readonly ConcurrentDictionary<Guid,
        ConcurrentDictionary<Guid, DateTime>> typing
        = new();

        public void StartTyping(Guid conversationId,Guid userId)
        {
            var users=typing.GetOrAdd(conversationId,
                _ => new ConcurrentDictionary<Guid, DateTime>());
            users[userId] = DateTime.UtcNow;

        }

        public void StopTyping(Guid conversationId,Guid userId)
        {
            if(typing.TryGetValue(conversationId,out var users))
            {
                users.TryRemove(userId, out _);
                if (users.IsEmpty)
                {
                    typing.TryRemove(conversationId,out _);
                }
            }
        }

        public List<Guid> GetTypingUsers(Guid conversationId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return [];

            return users.Keys.ToList();
        }

        public bool IsTyping(Guid conversationId, Guid userId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return false;

            return users.ContainsKey(userId);
        }
    }
}
