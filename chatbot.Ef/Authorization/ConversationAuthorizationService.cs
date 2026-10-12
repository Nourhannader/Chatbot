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
        public Task AuthorizeAsync(ClaimsPrincipal user, Guid conversationId, string permission)
        {
            return AuthorizeInternalAsync(user, new ConversationResource(conversationId), permission);
            
        }
      
        public  Task AuthorizeMessageAsync(ClaimsPrincipal user, Guid conversationId, Guid messageId, string permission)
        {
            return AuthorizeInternalAsync(user,new ConversationResource(conversationId,messageId:messageId), permission);
        }
    
        public Task AuthorizeMemberAsync(ClaimsPrincipal user,Guid conversationId,Guid targetUserId,string permission)
        {
            return AuthorizeInternalAsync(user,new ConversationResource(conversationId,targetUserId:targetUserId),permission);

        }
        
        private async Task AuthorizeInternalAsync(ClaimsPrincipal user,ConversationResource resource,string permission)
        {
            var requirement = new PermissionRequirement(permission);
            var result = await authorizationService.AuthorizeAsync(user, resource, requirement);

            if (!result.Succeeded)
                throw new ForbiddenException("You are not authorized to perform this action.");
        }
    }
}
