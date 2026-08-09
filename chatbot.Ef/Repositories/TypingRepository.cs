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
        private readonly ConcurrentDictionary<string,
        ConcurrentDictionary<string, DateTime>> typing
        = new();

        public void StartTyping(string conversationId,string userId)
        {
            var users=typing.GetOrAdd(conversationId,
                _ => new ConcurrentDictionary<string, DateTime>());
            users[userId] = DateTime.UtcNow;

        }

        public void StopTyping(string conversationId,string userId)
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

        public List<string> GetTypingUsers(string conversationId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return [];

            return users.Keys.ToList();
        }

        public bool IsTyping(string conversationId,string userId)
        {
            if (!typing.TryGetValue(conversationId, out var users))
                return false;

            return users.ContainsKey(userId);
        }
    }
}
