using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Events.Messages;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Outbox;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Authorization;

namespace chatbot.Ef.Services
{
    public class ForwardService(IUnitOfWork unitOfWork, IMapper mapper, OutboxMessageFactory outboxMessageFactory, IMessageStatusService messageStatusService,
        IAuthorizationService authorizationService, ICurrentUser CurrentUser) : IForwardService
    {
        public async Task<List<MessageDto>> ForwardAsync(Guid senderId, ForwardMessageDto dto)
        {
            //  Validate source message
            var source = await unitOfWork.Messages.GetWithDetailsAsync(dto.MessageId);
            if (source == null || source.IsDeletedForEveryone)
                throw new KeyNotFoundException("Message not found.");

            // Authorization on source message
            var sourceResource = new ConversationResource(source.ConversationId, messageId: source.Id);
            var sourceAuth = await authorizationService.AuthorizeAsync(CurrentUser.Principal, sourceResource, ConversationPermissions.ForwardMessage);

            if (!sourceAuth.Succeeded)
                throw new ForbiddenException("You cannot forward this message.");

            //check delete for Me
            var deletedForUser =
            await unitOfWork.Messages.IsDeletedForUserAsync(source.Id, senderId);

            if (deletedForUser)
            {
                throw new NotFoundException("Message not found.");
            }

            // Validate target conversations

            var conversationIds = dto.ConversationIds
                   .Where(x => x != Guid.Empty)
                   .Distinct()
                   .ToList();

            if (conversationIds.Count == 0)
            {
                throw new BadRequestException("At least one target conversation is required.");
            }


            var result = new List<Message>();

            var originalMessageId = source.IsForwarded ? source.OriginalMessageId ?? source.Id : source.Id;

            //create forward messages

            foreach (var conversationId in conversationIds)
            {
                var resource = new ConversationResource(conversationId);

                var authorizationResult =await authorizationService.AuthorizeAsync( CurrentUser.Principal, resource, ConversationPermissions.SendMessage);

                if (!authorizationResult.Succeeded)
                {
                    throw new ForbiddenException( $"Cannot send messages to conversation {conversationId}.");
                }

                var now = DateTime.UtcNow;

                var forwarded = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId =senderId,
                    ConversationId =conversationId,
                    Content = source.Content,
                    MessageType = source.MessageType,
                    IsForwarded =true,
                    OriginalMessageId = originalMessageId,
                    SendAt =now
                };


                CopyAttachments( source,forwarded);

                result.Add(forwarded);
            }
            //  Add all forwarded messages

            await unitOfWork.ForwardMessages.AddRangeAsync(result);

            // Recipients + Outbox Events

            foreach (var forwarded in result)
            {
                await messageStatusService.CreateRecipientsAsync(forwarded);

                var messageCreatedEvent =new MessageCreatedEvent(MessageId:forwarded.Id,ConversationId: forwarded.ConversationId,
                        SenderId: forwarded.SenderId, Content:forwarded.Content,MessageType: forwarded.MessageType, ReplyToMessageId:null,SentAt:forwarded.SendAt);

                await unitOfWork.OutboxMessages.AddAsync( outboxMessageFactory.Create(messageCreatedEvent));

                // Forward-specific event.
                var messageForwardedEvent = new MessageForwardedEvent(MessageId:forwarded.Id,OriginalMessageId: originalMessageId,
                        SourceConversationId:source.ConversationId, TargetConversationId:forwarded.ConversationId, ForwardedByUserId:senderId,ForwardedAt:forwarded.SendAt);

                await unitOfWork.OutboxMessages.AddAsync(outboxMessageFactory.Create(messageForwardedEvent));
            }

            // ONE COMMIT
            //
            // Forwarded Messages
            // Attachments
            // Recipient Statuses
            // MessageCreatedEvents
            // MessageForwardedEvents
            //
            // All saved atomically.
            // ==========================================

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<List<MessageDto>>(result);
        }

        private static void CopyAttachments( Message source,Message forwarded)
        {
            foreach (var attachment in source.Attachments)
            {
                forwarded.Attachments.Add(
                    new MessageAttachment
                    {
                        Id = Guid.NewGuid(),
                        StoredFileId =attachment.StoredFileId,
                        Order =attachment.Order
                    });
            }
        }

    }
}

 
