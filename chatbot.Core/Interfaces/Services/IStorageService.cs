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
        public  Task<UploadResultDto> UploadAsync(
      IFormFile file,
      string folder,
      Guid uploadedBy,
      Guid messageId,
      CancellationToken cancellationToken = default);

        Task<List<UploadResultDto>> UploadManyAsync(
            IEnumerable<IFormFile> files,
            Guid messageId,
            string folder,
            Guid uploadedBy,
            CancellationToken cancellationToken = default);

        Task<DownloadFileDto?> DownloadAsync(
            Guid fileId,
            CancellationToken cancellationToken = default);

        Task SoftDeleteAsync(Guid fileId,CancellationToken cancellationToken=default);

        Task<StoredFile?> GetByIdAsync(Guid fileId,CancellationToken cancellationToken=default);

        Task<string?> GetFileUrlAsync(Guid fileId,CancellationToken cancellationToken=default);
    }
}
