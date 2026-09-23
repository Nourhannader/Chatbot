using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IStorageService
    {
        Task<StoredFile?> GetByIdAsync(Guid fileId,
             CancellationToken cancellationToken = default);

        Task<UploadResultDto> UploadUserProfileImageAsync(IFormFile file, Guid userId,
            CancellationToken cancellationToken = default);

        Task<UploadResultDto> UploadConversationImageAsync(IFormFile file, Guid conversationId, Guid uploadedBy,
            CancellationToken cancellationToken = default);

        Task<UploadResultDto> UploadMessageFileAsync(IFormFile file, Guid messageId, Guid uploadedBy,
            CancellationToken cancellationToken = default);

        Task<List<UploadResultDto>> UploadMessageFilesAsync(IEnumerable<IFormFile> files, Guid messageId, Guid uploadedBy,
            CancellationToken cancellationToken = default);

        Task<UploadResultDto> ReplaceUserProfileImageAsync(IFormFile newFile, Guid userId, Guid? oldFileId,
            CancellationToken cancellationToken = default);

        Task<UploadResultDto> ReplaceConversationImageAsync(IFormFile newFile, Guid conversationId, Guid uploadedBy, Guid? oldFileId,
            CancellationToken cancellationToken = default);

        Task<DownloadFileDto?> DownloadAsync(Guid fileId,
            CancellationToken cancellationToken = default);

        Task<string?> GetFileUrlAsync(Guid fileId,
            CancellationToken cancellationToken = default);

        Task SoftDeleteAsync(Guid fileId,
            CancellationToken cancellationToken = default);

        Task<bool> DeletePhysicallyAsync(Guid fileId,
            CancellationToken cancellationToken = default);
    }
}
