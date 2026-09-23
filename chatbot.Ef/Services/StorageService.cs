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
using chatbot.Core.Exceptions;

namespace chatbot.Ef.Services
{
    public class StorageService(IWebHostEnvironment environment,IUnitOfWork unitOfWork,
        IFileValidationService validators,IEnumerable<IStorageProvider> providers) : IStorageService
    {
        private static string GeneratePath(string folder,string fileName)
        {
            var extension =Path.GetExtension(fileName).ToLowerInvariant();

            var now = DateTime.UtcNow;

            var uniqueFileName =$"{Guid.NewGuid()}{extension}";

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
        private static string GetFolder(FileCategory category,Guid ownerId)
        {
            return category switch
            {
                FileCategory.UserProfileImage =>
                    $"images/users/{ownerId}",

                FileCategory.ConversationImage =>
                    $"images/conversations/{ownerId}",

                FileCategory.MessageFile =>
                    $"messages/{ownerId}",

                _ => throw new ArgumentOutOfRangeException(
                    nameof(category))
            };
        }

        private async Task<UploadResultDto> UploadInternalAsync(IFormFile file, FileCategory category,
            Guid ownerId, Guid? uploadedBy, Guid? messageId, CancellationToken cancellationToken)
        {
            // Validation
            if (category == FileCategory.UserProfileImage ||
                category == FileCategory.ConversationImage)
            {
                await validators.ValidateImageAsync(file);
            }
            else
            {
                await validators.ValidateFileAsync(file);
            }
            cancellationToken.ThrowIfCancellationRequested();
            var provider = GetProvider(StorageProviderType.Local);
            var folder = GetFolder(category, ownerId);
            var relativePath = GeneratePath(folder, file.FileName);


            try
            {
                // Physical upload
                await using var stream = file.OpenReadStream();

                await provider.UploadAsync(stream, relativePath, file.ContentType, cancellationToken);

                // DB entity
                var storedFile =
                    new StoredFile
                    {
                        Id = Guid.NewGuid(),

                        OriginalName = Path.GetFileName(file.FileName),

                        StoredName = Path.GetFileName(relativePath),

                        Path = relativePath,

                        ContentType = file.ContentType,

                        Size = file.Length,

                        Category = category,

                        Provider = provider.ProviderType,

                        UploadedByUserId = uploadedBy,

                        MessageId = messageId,

                        IsDeleted = false,

                        IsPhysicallyDeleted = false,

                        CreatedAt = DateTime.UtcNow
                    };

                await unitOfWork.StoredFiles.AddAsync(storedFile);

                await unitOfWork.SaveChangesAsync();

                return new UploadResultDto
                {
                    Success = true,

                    FileId = storedFile.Id,

                    FileName = storedFile.OriginalName,

                    FileUrl = provider.GetFileUrl(storedFile.Path),

                    ContentType = storedFile.ContentType,

                    Size = storedFile.Size
                };
            }
            catch
            {
                // If DB failed after physical upload,
                // try deleting the physical file.
                try
                {
                    await provider.DeleteAsync(relativePath, CancellationToken.None);
                }
                catch
                {
                    // Don't hide original exception.
                }

                throw;
            }
        }
        public async Task<StoredFile?> GetByIdAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await unitOfWork.StoredFiles
                .GetByIdAsync(fileId);
        }
        // ============================
        // USER PROFILE IMAGE
        // ============================

        public async Task<UploadResultDto> UploadUserProfileImageAsync(IFormFile file, Guid userId,
                CancellationToken cancellationToken = default)
        {
            return await UploadInternalAsync(file, FileCategory.UserProfileImage, userId, userId, null, cancellationToken);
        }

        public async Task<UploadResultDto> ReplaceUserProfileImageAsync(IFormFile newFile, Guid userId, Guid? oldFileId,
                CancellationToken cancellationToken = default)
        {
            // Upload new file first
            var result =
                await UploadUserProfileImageAsync(newFile, userId, cancellationToken);

            // Delete old file after successful upload
            if (oldFileId.HasValue)
            {
                await DeletePhysicallyAsync(oldFileId.Value, cancellationToken);
            }

            return result;
        }

        // ============================
        // CONVERSATION IMAGE
        // ============================

        public async Task<UploadResultDto> UploadConversationImageAsync(IFormFile file, Guid conversationId, Guid uploadedBy,
                CancellationToken cancellationToken = default)
        {
            return await UploadInternalAsync(file, FileCategory.ConversationImage, conversationId, uploadedBy, null, cancellationToken);
        }

        public async Task<UploadResultDto> ReplaceConversationImageAsync(IFormFile newFile, Guid conversationId, Guid uploadedBy, Guid? oldFileId,
                CancellationToken cancellationToken = default)
        {
            var result = await UploadConversationImageAsync(newFile, conversationId, uploadedBy, cancellationToken);

            if (oldFileId.HasValue)
            {
                await DeletePhysicallyAsync(oldFileId.Value, cancellationToken);
            }

            return result;
        }

        // ============================
        // MESSAGE FILE
        // ============================

        public async Task<UploadResultDto> UploadMessageFileAsync(IFormFile file, Guid messageId, Guid uploadedBy,
                CancellationToken cancellationToken = default)
        {
            return await UploadInternalAsync(file, FileCategory.MessageFile, messageId, uploadedBy, messageId, cancellationToken);
        }

        public async Task<List<UploadResultDto>> UploadMessageFilesAsync(IEnumerable<IFormFile> files, Guid messageId, Guid uploadedBy,
                CancellationToken cancellationToken = default)
        {
            var results = new List<UploadResultDto>();

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result =
                    await UploadMessageFileAsync(file, messageId, uploadedBy, cancellationToken);

                results.Add(result);
            }

            return results;
        }

        // ============================
        // DOWNLOAD
        // ============================

        public async Task<DownloadFileDto?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);

            if (storedFile == null ||
                storedFile.IsDeleted ||
                storedFile.IsPhysicallyDeleted)
            {
                return null;
            }

            var provider = GetProvider(storedFile.Provider);

            var stream = await provider.DownloadAsync(storedFile.Path, cancellationToken);

            if (stream == null)
                return null;

            return new DownloadFileDto
            {
                stream = stream,

                FileName = storedFile.OriginalName,

                ContentType =
                    string.IsNullOrWhiteSpace(
                        storedFile.ContentType)
                    ? "application/octet-stream"
                    : storedFile.ContentType
            };
        }

        // ============================
        // URL
        // ============================

        public async Task<string?> GetFileUrlAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);

            if (storedFile == null ||
                storedFile.IsDeleted ||
                storedFile.IsPhysicallyDeleted)
            {
                return null;
            }

            var provider = GetProvider(storedFile.Provider);

            return provider.GetFileUrl(storedFile.Path);
        }
        // ============================
        // SOFT DELETE
        // ============================

        public async Task SoftDeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);

            if (storedFile == null)
                throw new KeyNotFoundException("File not found.");

            if (storedFile.IsDeleted)
                return;

            storedFile.IsDeleted = true;

            storedFile.DeletedAt = DateTime.UtcNow;

           await unitOfWork.StoredFiles.UpdateAsync(storedFile);

            await unitOfWork.SaveChangesAsync();
        }

        // ============================
        // PHYSICAL DELETE
        // ============================

        public async Task<bool> DeletePhysicallyAsync(Guid fileId, CancellationToken cancellationToken = default)
        {
            var storedFile = await unitOfWork.StoredFiles.GetByIdAsync(fileId);

            if (storedFile == null)
                return false;

            if (storedFile.IsPhysicallyDeleted)
                return true;

            var provider = GetProvider(storedFile.Provider);

            await provider.DeleteAsync(storedFile.Path, cancellationToken);

            storedFile.IsDeleted = true;

            storedFile.IsPhysicallyDeleted = true;

            storedFile.DeletedAt = DateTime.UtcNow;

            await unitOfWork.StoredFiles.UpdateAsync(storedFile);

            await unitOfWork.SaveChangesAsync();

            return true;
        }

    }
    
}
