using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Validators;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class StorageService(IWebHostEnvironment environment,IUnitOfWork unitOfWork,
        IFileValidationService validators,IFileProcessorService processor,
        IEnumerable<IStorageProvider> providers) : IStorageService
    {
        private static string GeneratePath(
        string folder,
        string fileName)
        {
            var extension =
                Path.GetExtension(fileName)
                    .ToLowerInvariant();

            var now = DateTime.UtcNow;

            var uniqueFileName =
                $"{Guid.NewGuid()}{extension}";

            return Path.Combine(
                folder,
                now.Year.ToString(),
                now.Month.ToString("00"),
                now.Day.ToString("00"),
                uniqueFileName)
                .Replace("\\", "/");
        }
        private IStorageProvider GetProvider(StorageProviderType providerType)
        {
            return providers.First(x =>
                x.ProviderType == providerType)??
                throw new InvalidOperationException($"Storage provider '{providerType}' is not registered.");
        }

        public async Task<StoredFile?> GetByIdAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await unitOfWork.StoredFiles.GetByIdAsync(fileId);
        }
        public async Task SoftDeleteAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);

            if (storedFile == null)
            {
                throw new KeyNotFoundException(
                    "File not found.");
            }
            if (storedFile.IsDeleted)
                return;

            storedFile.IsDeleted = true;
            storedFile.DeletedAt = DateTime.UtcNow;
            unitOfWork.StoredFiles.UpdateAsync(storedFile);
        }

        public async Task<DownloadFileDto?> DownloadAsync(Guid fileId,CancellationToken cancellationToken=default)
        {
            var storedFile=await unitOfWork.StoredFiles.GetByIdAsync(fileId);
            if (storedFile == null || storedFile.IsDeleted || storedFile.IsPhysicallyDeleted)
                return null;

            cancellationToken.ThrowIfCancellationRequested();
            var provider = GetProvider(storedFile.StorageProvider);

            var stream = await provider.DownloadAsync(storedFile.Path,cancellationToken);
            if (stream == null)
                return null;
            return new DownloadFileDto
            {
                stream = stream,
                FileName = storedFile.OriginalName,
                ContentType = string.IsNullOrWhiteSpace(
                    storedFile.ContentType)
                ? "application/octet-stream"
                : storedFile.ContentType
            };

        }

        public async Task<UploadResultDto> UploadAsync(
       IFormFile file,
       string folder,
       Guid uploadedBy,
       Guid messageId,
       CancellationToken cancellationToken = default)
        {
            //validation
            await validators.ValidateFile(file);
            cancellationToken.ThrowIfCancellationRequested();
            //select provider
            var provider=GetProvider(StorageProviderType.Local);
            //generate unique path
            var relativePath = GeneratePath(folder, file.FileName);

            //upload physical file
            await using var stream=file.OpenReadStream();

            var storedFile = new StoredFile
            {
                Id = Guid.NewGuid(),

                OriginalName = Path.GetFileName(
                file.FileName),

                StoredName = Path.GetFileName(
                relativePath),

                Path = relativePath,

                ContentType = file.ContentType,

                Size = file.Length,

                MessageId = messageId,

                UploadedByUserId =uploadedBy,

                StorageProvider = provider.ProviderType,

                IsDeleted = false,

                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.StoredFiles.AddAsync(storedFile);

            return new UploadResultDto
            {
                Success = true,

                FileId = storedFile.Id,

                FileUrl = provider.GetFileUrl(
                 storedFile.Path),

                ContentType = storedFile.ContentType,

                Size = storedFile.Size
            };
        }

        public async Task<List<UploadResultDto>> UploadManyAsync(
        IEnumerable<IFormFile> files,
        Guid messageId,
        string folder,
        Guid uploadedBy,
        CancellationToken cancellationToken = default)
        {
            var results = new List<UploadResultDto>();

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await UploadAsync(
                    file,
                    folder,
                    uploadedBy,
                    messageId,
                    cancellationToken);

                results.Add(result);
            }

            return results;
        }

        public async Task<string?> GetFileUrlAsync(Guid fileId,CancellationToken cancellationToken=default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);
            if (storedFile == null || storedFile.IsDeleted || storedFile.IsPhysicallyDeleted)
                return null;

            var provider = GetProvider(storedFile.StorageProvider);
            return provider.GetFileUrl(storedFile.Path);
        }

        
    }
    
}
