using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        public ChatService(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<Message> CreateMessage(int chatId, string senderId, string text)
        {
            var msg = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                MessageText = text,
                MessageType = MessageType.Text,
                SentAt = DateTime.UtcNow


            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();
            return msg;
        }

        public async Task<bool> IsBlocked(int chatId, string senderId)
        {
            var ChatUser=_context.ChatMembers
                .Where(cm => cm.ChatId == chatId)
                .Select(cm => cm.UserId)
                .ToList();

           return await _context.BlockLists.AnyAsync(bl => bl.BlockedId == senderId && ChatUser.Contains(bl.BlockerId));
        }
        

        public async Task<bool> IsMember(int chatId, string userId) =>
           await _context.ChatMembers.AnyAsync(cm => cm.ChatId == chatId && cm.UserId == userId);

        

        public async Task SetUserOffline(string connectionId)
        {
            var userOnline = _context.UserDevices.FirstOrDefault(ou => ou.ConnectionId == connectionId);
            if(userOnline != null)
            {
                userOnline.IsConnected = false;
                userOnline.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetUserOnline(string userId, string connectionId)
        {
            var device = new UserDevice
            {
                UserId = userId,
                ConnectionId = connectionId,
                IsConnected = true,
                LastSeen = DateTime.UtcNow
            };
            _context.UserDevices.Add(device);
            await _context.SaveChangesAsync();
        }
    }
}
