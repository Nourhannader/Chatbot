using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cmp;

namespace chatbot.Ef.Authorization
{
    public class ConversationAuthorizationService(IAuthorizationService authorizationService,IUnitOfWork unitOfWork) : IConversationAuthorizationService
    {
        public async Task AuthorizeAsync(ClaimsPrincipal user, Guid conversationId, string permission)
        {
            
            var resource = new ConversationResource(conversationId);
           await AuthorizeAsync(user, resource, permission);
            
        }

        public async Task AuthorizeAsync(ClaimsPrincipal user, ConversationResource resource, string permission)
        {
            var requirement = new PermissionRequirement(permission);
            var result = await authorizationService.AuthorizeAsync(
                user, resource, requirement);

            if (!result.Succeeded)
            {
                throw new ForbiddenException("You are not authorized to perform this action.");
            }
        }

        public async Task AuthorizeMessageAsync(ClaimsPrincipal user, Guid conversationId, Guid messageId, string permission)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new ForbiddenException("User is not authenticated.");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new ForbiddenException("Invalid user identity.");
            }
            
            var message = await unitOfWork.Messages.GetMessageByConversationIdAsync(messageId, conversationId);

            if (message == null)
            {
                throw new NotFoundException("Message not found.");
            }

            var member = await unitOfWork.ConversationMember.GetAsync(conversationId, userId);
               

            if (member == null)
            {
                throw new ForbiddenException("You are not a member of this conversation.");
            }

            var isOwner = member.Role == ConversationRole.Owner;

            var isAdmin = member.Role == ConversationRole.Admin;

            var isSender = message.SenderId == userId;

            if (isOwner || isAdmin)
                return;

            if (isSender && permission == ConversationPermissions.DeleteMessage)
            {
                return;
            }

            throw new ForbiddenException("You are not authorized to perform this action.");
        }
    }
}
