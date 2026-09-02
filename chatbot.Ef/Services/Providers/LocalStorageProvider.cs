using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace chatbot.Ef.Services.Providers
{
    public class LocalStorageProvider(IWebHostEnvironment environment) : IStorageProvider
    {
        public StorageProviderType ProviderType => StorageProviderType.Local;

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(environment.WebRootPath, "uploads", path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(environment.WebRootPath, "uploads", path);
            return Task.FromResult(File.Exists(fullPath));
        }

        public string GetFileUrl(string path)
        {
            return $"/uploads/{path.Replace("\\", "/")}";
        }

        public async Task<string> UploadAsync(Stream stream, string path, string contentType, CancellationToken cancellationToken = default)
        {
            var FullPath=Path.Combine(environment.WebRootPath, "uploads", path);
            var directory = Path.GetDirectoryName(FullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var fileStream = new FileStream(FullPath, FileMode.Create,FileAccess.Write);
            await stream.CopyToAsync(fileStream, cancellationToken);

            return path;
        }
    }
}
