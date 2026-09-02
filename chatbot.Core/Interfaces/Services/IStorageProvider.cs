using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services
{
    public interface IStorageProvider
    {
        StorageProviderType ProviderType { get; }
        Task<string> UploadAsync(Stream stream, string path, string contentType, CancellationToken cancellationToken = default);
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
        string GetFileUrl(string path);


    }
}
