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
        public async Task AddMemberAsync(Guid conversationId, Guid userId)
        {
            var conversation = await unitOfWork.Conversations.GetByIdAsync(conversationId);
            if(conversation == null)
                throw new Exception("Conversation not found.");
            
            if (conversation.Members.Any(m => m.UserId == userId)) return;

            conversation.Members.Add(new ConversationMember
            {
                UserId = userId,
                ConversationId = conversationId
            });

             unitOfWork.Conversations.Update(conversation);
            await unitOfWork.SaveChangesAsync();
        }

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
                Type = ConversationType.OneToOne
            };
            conversation.Members.Add(new ConversationMember
            {
                UserId = creatorId,
                ConversationId = conversation.Id
            });
            conversation.Members.Add(new ConversationMember
            {
                UserId = secondUserId,
                ConversationId = conversation.Id
            });
            await unitOfWork.Conversations.AddAsync(conversation);
            await unitOfWork.SaveChangesAsync();
            return conversation;

        }

        public async Task<Conversation> CreateGroupAsync(Guid creatorId, string title, List<string> members)
        {
            var group = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Group,
                Title = title
            };
            group.Members.Add(new ConversationMember
            {
                UserId = creatorId,
                ConversationId = group.Id,
                IsAdmin = true
            });

            foreach (var member in members.Distinct())
            {
                if (Guid.TryParse(member, out Guid memberGuid) && memberGuid == creatorId) continue;
                group.Members.Add(new ConversationMember
                {
                    UserId = memberGuid, 
                    ConversationId = group.Id
                });
            }

            await unitOfWork.Conversations.AddAsync(group);
            await unitOfWork.SaveChangesAsync();

            return group;
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await unitOfWork.Conversations.GetUserConversationsAsync(userId);
        }

        public async Task RemoveMemberAsync(Guid conversationId, Guid userId)
        {
            var conversation = await unitOfWork.Conversations.GetByIdAsync(conversationId);
            if(conversation == null)
                throw new Exception("Conversation not found.");

            var member = conversation.Members.FirstOrDefault(m => m.UserId == userId);

            if (member == null)
                return;

            conversation.Members.Remove(member);
            unitOfWork.Conversations.Update(conversation);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
