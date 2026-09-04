using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IChunkUploadService
    {
        Task<Guid> StartAsync(
            StartUploadDto dto,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task UploadChunkAsync(
            Guid sessionId,
            int chunkNumber,
            IFormFile chunk,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<UploadResultDto> CompleteAsync(
            Guid sessionId,
            Guid messageId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
