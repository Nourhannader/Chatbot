using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services.Helper
{
    public class PermissionService(IUnitOfWork unitOfWork)
    {
        public async Task<ConversationMember> GetActiveMemberAsync(Guid coversationId ,Guid userId)
        {
            var member = await unitOfWork.ConversationMember.GetAsync(coversationId, userId);
            if (member == null || member.LeftAt !=null)
            {
                throw new UnauthorizedAccessException("User is not an active member.");
            }
            return member;
        }
        public async Task<ConversationMember>RequireAdminAsync(Guid conversationId,Guid userId)
        {
            var member = await GetActiveMemberAsync(conversationId, userId);

            if (member.Role != GroupRole.Admin &&
                member.Role != GroupRole.Owner)
            {
                throw new UnauthorizedAccessException(
                    "Admin permission required.");
            }

            return member;
        }
        public async Task<ConversationMember> RequireOwnerAsync(Guid conversationId, Guid userId)
        {
            var member = await GetActiveMemberAsync(conversationId, userId);

            if (member.Role != GroupRole.Owner)
            {
                throw new UnauthorizedAccessException(
                    "Admin permission required.");
            }

            return member;
        }

    }
}
