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

        private string GetRootPath()
        {
            if (!string.IsNullOrWhiteSpace(environment.WebRootPath))
            {
                return environment.WebRootPath;
            }
            return Path.Combine(environment.ContentRootPath, "wwwroot");
        }
        private string GetFullPath(string path)
        {
            var root = GetRootPath();
            return Path.Combine(root,"uploads", path);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fullPath = GetFullPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }

        public Task<Stream?> DownloadAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fullPath = GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(null);
            }
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream?>(stream);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fullPath = GetFullPath(path);
            return Task.FromResult(File.Exists(fullPath));
        }

        public string GetFileUrl(string path)
        {
            return $"/uploads/{path.Replace("\\", "/")}";
        }

        public async Task UploadAsync(Stream stream, string path, string contentType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var FullPath=GetFullPath(path);
            var directory = Path.GetDirectoryName(FullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var fileStream = new FileStream(FullPath, FileMode.Create,FileAccess.Write,FileShare.None);
            await stream.CopyToAsync(fileStream, cancellationToken);

            
        }
    }
}
