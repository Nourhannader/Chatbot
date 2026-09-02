using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Services.Providers
{
    //skeleton class for Azure Blob Storage provider, to be implemented in the future
    public class AzureBlobStorageProvider : IStorageProvider
    {
        public StorageProviderType ProviderType => StorageProviderType.AzureBlob;

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            //delete blob
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            // Check blob

            return Task.FromResult(false);
        }

        public string GetFileUrl(string path)
        {
            // Azure CDN or Blob URL

            return path;
        }

        public Task<string> UploadAsync(Stream stream, string path, string contentType, CancellationToken cancellationToken = default)
        {
            // Upload blob
            return Task.FromResult(path);
        }
    }
}
