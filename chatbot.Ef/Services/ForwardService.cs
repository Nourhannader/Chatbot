using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class ForwardService(IUnitOfWork unitOfWork,IMapper mapper) : IForwardService
    {
        public async Task<List<MessageDto>> ForwardAsync(Guid senderId, ForwardMessageDto dto)
        {
            var original = await unitOfWork.Messages.GetWithDetailsAsync(dto.MessageId);
            if(original ==null || original.IsDeletedForEveryone)
                throw new KeyNotFoundException("Message not found.");

            var sourceMember = await unitOfWork.ConversationMember
                .ExistsAsync(original.ConversationId, senderId);

            if (!sourceMember)
            {
                throw new UnauthorizedAccessException("You do not have access to the original message.");
            }

            var deletedForUser = await unitOfWork.Messages
                .IsDeletedForUserAsync(original.Id, senderId);

            if (deletedForUser)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            //get valid target conversations 
            var validConversationIds = await unitOfWork.ForwardMessages
                .GetValidConversationIdsAsync(senderId, dto.ConversationIds);


            if (validConversationIds.Count == 0)
            {
                throw new InvalidOperationException("No valid target conversations were found.");
            }

            //create forwarded messages

            var forwardedMessages = new List<Message>();
            foreach (var conversationId in validConversationIds)
            {
                // THIS is forwardedMessage
                var forwardedMessage =
                    new Message
                    {
                        Id = Guid.NewGuid(),

                        SenderId =senderId,

                        ConversationId = conversationId,

                        Content =original.Content,

                        MessageType = original.MessageType,

                        OriginalMessageId =original.Id,

                        IsForwarded = true,

                        SendAt = DateTime.UtcNow
                    };


                // Copy attachment REFERENCES
                foreach (var attachment in original.Attachments)
                {
                    forwardedMessage.Attachments.Add(
                        new MessageAttachment
                        {
                            Id = Guid.NewGuid(),

                            StoredFileId =attachment.StoredFileId,

                            Order =attachment.Order,

                            Caption =attachment.Caption
                        });
                }


                //  Add to collection
                forwardedMessages.Add(forwardedMessage);
            }


            await unitOfWork.ForwardMessages.AddRangeAsync(forwardedMessages);
            await unitOfWork.SaveChangesAsync();

       
            return mapper.Map<List<MessageDto>>(forwardedMessages);

        }
    }
}
