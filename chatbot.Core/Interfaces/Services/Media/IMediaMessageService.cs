using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMediaMessageService
    {
        Task<MessageDto> SendMediaAsync(SendMediaDto dto, Guid uploadBy,CancellationToken cancellationToken = default);

        //Task<MediaDownloadResultDto> GetAttachmentAsync(Guid messageId,Guid attachmentId, Guid userId,CancellationToken cancellationToken = default);
    }
}
