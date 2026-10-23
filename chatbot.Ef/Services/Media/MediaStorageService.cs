using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services.Media
{
    public class MediaStorageService(IStorageService storageService) : IMediaStorageService
    {
        public async Task DeleteAsync(StoredFile file, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);
            return await storageService.DeleteAsync(file.StorageKey, cancellationToken);
        }

        public async Task<StoredFile> UploadAsync(IFormFile file, ValidatedFileDto validatedFile, Guid uploadedById, string folder, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(validatedFile);

            var storedFileName = $"{Guid.NewGuid():N}{validatedFile.Extension}";

            await using var stream = file.OpenReadStream();

            var uploadResult = await storageService.UploadAsync(stream, storedFileName,
                    validatedFile.ContentType, folder, cancellationToken);

            return new StoredFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = validatedFile.OriginalFileName,
                StoredFileName = storedFileName,
                Extension = validatedFile.Extension,
                ContentType = validatedFile.ContentType,
                FileSize = validatedFile.FileSize,
                Category = validatedFile.Category,
                Provider = uploadResult.Provider,
                StorageKey = uploadResult.StorageKey,
                UploadedById = uploadedById,
                CreatedAt = DateTime.UtcNow,
                ProcessingStatus = RequiresProcessing(validatedFile.Category)
                        ? FileProcessingStatus.Pending : FileProcessingStatus.Completed,
                ProcessingCompletedAt = RequiresProcessing(validatedFile.Category)
                        ? null : DateTime.UtcNow
            };
        }

        private static bool RequiresProcessing(FileCategory category)
        {
            return category is
                FileCategory.Image or
                FileCategory.Video or
                FileCategory.Audio;
        }
    }
}
