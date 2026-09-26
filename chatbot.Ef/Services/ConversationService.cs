using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class ConversationService(IUnitOfWork unitOfWork) : IConversationService
    {
     //private conversation   
        public async Task<Conversation> CreateConversationAsync(Guid creatorId, Guid secondUserId)
        {
            bool exists = await unitOfWork.Conversations.ConversationExistsAsync(creatorId, secondUserId);
            if (exists)
            {
                throw new Exception("Conversation already exists.");
            }
            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Private,
                CreatedById=creatorId,
                CreatedAt=DateTime.UtcNow
            };
            conversation.Members.Add(new ConversationMember
            {
                Id = Guid.NewGuid(),
                UserId = creatorId,
                ConversationId = conversation.Id,
                Role = ConversationRole.Member,
                JoinedAt = DateTime.UtcNow
            });
            conversation.Members.Add(new ConversationMember
            {
                Id = Guid.NewGuid(),
                UserId = secondUserId,
                ConversationId = conversation.Id,
                Role = ConversationRole.Member,
                JoinedAt = DateTime.UtcNow
            });
            await unitOfWork.Conversations.AddAsync(conversation);
            await unitOfWork.SaveChangesAsync();
            return conversation;

        }

        public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await unitOfWork.Conversations.GetUserConversationsAsync(userId);
        }

       
    }
}
