using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Events.Conversations;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Outbox;

namespace chatbot.Ef.Services
{
    public sealed class ConversationMemberService(IUnitOfWork unitOfWork, IConversationAuthorizationService authorizationService
        , IMapper mapper, TimeProvider timeProvider,OutboxMessageFactory outboxMessageFactory) : IConversationMemberService
    {
        public async Task<ConversationMemberDetailsDto> GetCurrentMemberAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.View);

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, user.GetUserId())
                ?? throw new NotFoundException("Membership not found.");

            return mapper.Map<ConversationMemberDetailsDto>(member);
        }

        public async Task<List<ConversationMemberDto>> GetMembersAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.View);

            var members = await unitOfWork.ConversationMember
                    .GetActiveMembersAsync(conversationId);

            return mapper.Map<List<ConversationMemberDto>>(members);
        }

        public async Task MarkAsReadAsync(ClaimsPrincipal user, Guid conversationId, Guid? lastReadMessageId = null)
        {
            await authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.View);

            var userId = user.GetUserId();

            var member = await unitOfWork.ConversationMember
                    .GetActiveAsync(conversationId, userId)
                ?? throw new NotFoundException("Membership not found.");

            var now = timeProvider.GetUtcNow().UtcDateTime;

            member.LastReadAt = now;

            // If you add LastReadMessageId later:
            // member.LastReadMessageId = lastReadMessageId;

            var domainEvent = new ConversationReadEvent( ConversationId: conversationId, UserId: userId, ReadAt: now);
            await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(domainEvent));

            await unitOfWork.SaveChangesAsync();

        }
    }
}
