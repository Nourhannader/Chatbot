using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IStorageService
    {
        Task<UploadResultDto> UploadAsync(IFormFile file,string folder,string uploadedBy,StorageProviderType providerType=StorageProviderType.Local);

        Task DeleteAsync(Guid fileId);

        Task<Stream> DownloadAsync(string fileUrl);

        Task<string?> GetFileUrlAsync(Guid fileId);
    }
}
