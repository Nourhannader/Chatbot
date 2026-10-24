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
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Services
{
    public class StorageService(IUnitOfWork unitOfWork,ILogger<StorageService> logger, IEnumerable<IStorageProvider> providers) : IStorageService
    {

        // =====================================
        // Get Provider
        // =====================================

        private IStorageProvider GetProvider(StorageProviderType providerType)
        {
            var provider = providers.SingleOrDefault(x => x.ProviderType == providerType);

            return provider ?? throw new InvalidOperationException($"Storage provider '{providerType}' is not registered.");
        }

        // =====================================
        // Generate Storage Key
        // =====================================

        private static string GenerateStorageKey(string folder, string extension)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("Storage folder is required.", nameof(folder));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("File extension is required.", nameof(extension));
            }

            extension = extension.Trim().ToLowerInvariant();

            if (!extension.StartsWith('.'))
            {
                extension = $".{extension}";
            }

            var now = DateTime.UtcNow;

            var fileName = $"{Guid.NewGuid():N}{extension}";

            return
                $"{folder.TrimEnd('/')}/" +
                $"{now:yyyy/MM/dd}/" +
                $"{fileName}";
        }

        private static FileProcessingStatus GetInitialProcessingStatus(FileCategory category, MediaType mediaType)
        {
            if (category == FileCategory.Thumbnail)
            {
                return FileProcessingStatus.Completed;
            }

            return mediaType switch
            {
                MediaType.Image => FileProcessingStatus.Pending,

                MediaType.Video => FileProcessingStatus.Pending,

                MediaType.Audio => FileProcessingStatus.Pending,

                MediaType.Document => FileProcessingStatus.NotRequired,

                MediaType.Other => FileProcessingStatus.NotRequired,

                _ => FileProcessingStatus.NotRequired
            };
        }


        //=============================
        //folder
        //=============================
        private static string GetFolder( FileCategory category,Guid? ownerId = null)
        {
            return category switch
            {
                FileCategory.UserProfileImage
                    when ownerId.HasValue &&
                         ownerId.Value != Guid.Empty
                        => $"images/users/{ownerId.Value}",


                FileCategory.ConversationImage
                    when ownerId.HasValue &&
                         ownerId.Value != Guid.Empty
                        => $"images/conversations/{ownerId.Value}",


                FileCategory.MessageFile
                    when ownerId.HasValue &&
                         ownerId.Value != Guid.Empty
                        => $"messages/{ownerId.Value}",


                // Thumbnail doesn't need a business owner folder.
                FileCategory.Thumbnail
                    => "messages/thumbnails",


                FileCategory.UserProfileImage or
                FileCategory.ConversationImage or
                FileCategory.MessageFile
                    => throw new ArgumentException($"Owner id is required for file category '{category}'.",nameof(ownerId)),


                _ => throw new ArgumentOutOfRangeException(nameof(category), category,"Unsupported file category.")
            };
        }

        //==============================
        //upload
        //===============================
        public async Task<StoredFile> UploadAsync(IFormFile file, ValidatedFileDto validatedFile, FileCategory category,
                     Guid ownerId, Guid? uploadedById, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(validatedFile);

            if (ownerId == Guid.Empty)
            {
                throw new ArgumentException("Owner id cannot be empty.", nameof(ownerId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var provider = GetProvider(StorageProviderType.Local);

            var folder = GetFolder(category, ownerId);

            var storageKey = GenerateStorageKey(folder, validatedFile.Extension);

            var uploaded = false;

            try
            {
                await using var stream = file.OpenReadStream();

                await provider.UploadAsync(stream, storageKey, validatedFile.ContentType, cancellationToken);

                uploaded = true;

                var status = GetInitialProcessingStatus(category, validatedFile.MediaType);

                var storedFile =
                    new StoredFile
                    {
                        Id = Guid.NewGuid(),
                        OriginalFileName = validatedFile.OriginalFileName,
                        StoredFileName = Path.GetFileName(storageKey),
                        Extension = validatedFile.Extension,
                        ContentType = validatedFile.ContentType,
                        FileSize = validatedFile.FileSize,
                        Category = category,
                        MediaType = validatedFile.MediaType,
                        Provider = provider.ProviderType,
                        StorageKey = storageKey,
                        UploadedById = uploadedById,
                        CreatedAt = DateTime.UtcNow,
                        ProcessingStatus = status,
                        ProcessingCompletedAt =
                            status is
                                FileProcessingStatus.Completed or
                                FileProcessingStatus.NotRequired
                                    ? DateTime.UtcNow
                                    : null,
                        ProcessingStartedAt = null,
                        ProcessingError=null,
                        IsDeleted = false,
                        DeletedAt = null,
                        IsPhysicallyDeleted = false
                    };

                await unitOfWork.StoredFiles.AddAsync(storedFile);
                await unitOfWork.SaveChangesAsync();
                return storedFile;
            }
            catch
            {
                // -----------------------------------------
                // Compensation
                // -----------------------------------------
                //
                // Physical upload may succeed,
                // while database save fails.
                //

                if (uploaded)
                {
                    await TryCleanupAsync(provider, storageKey);
                }

                throw;
            }

        }
        
        // =====================================
        // Upload Generated File
        // Thumbnail etc.
        // =====================================

        public async Task<StoredFile> UploadGeneratedAsync(Stream stream, string fileName, string contentType,
            FileCategory category, MediaType mediaType,
            Guid? uploadedById, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream must be readable.", nameof(stream));
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required.", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException("Content type is required.", nameof(contentType));
            }

            var safeFileName = Path.GetFileName(fileName);

            var extension = Path.GetExtension(safeFileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Generated file extension is required.", nameof(fileName));
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var provider = GetProvider(StorageProviderType.Local);
            var folder =
            category switch
            {
                FileCategory.Thumbnail =>
                    GetFolder(
                        FileCategory.Thumbnail),

                _ =>
                    throw new ArgumentException(
                        $"Generated file category '{category}' is not supported.",
                        nameof(category))
            };

            var storageKey = GenerateStorageKey(folder, extension);

            var uploaded = false;

            try
            {
                await provider.UploadAsync(stream, storageKey, contentType, cancellationToken);

                uploaded = true;
                var status = GetInitialProcessingStatus(category, mediaType);

                var storedFile =
                    new StoredFile
                    {
                        Id = Guid.NewGuid(),
                        OriginalFileName = safeFileName,
                        StoredFileName = Path.GetFileName(storageKey),
                        Extension = extension,
                        ContentType = contentType,
                        FileSize = stream.CanSeek ? stream.Length : 0,
                        Category = category,
                        MediaType = mediaType,
                        Provider = provider.ProviderType,
                        StorageKey = storageKey,
                        UploadedById = uploadedById,
                        CreatedAt = DateTime.UtcNow,
                        ProcessingStatus = status,
                        ProcessingCompletedAt =
                            status is
                                FileProcessingStatus.Completed or
                                FileProcessingStatus.NotRequired
                                    ? DateTime.UtcNow
                                    : null,
                        ProcessingStartedAt =null,
                        ProcessingError=null,
                        IsDeleted = false,
                        IsPhysicallyDeleted = false
                    };

                await unitOfWork.StoredFiles.AddAsync(storedFile);

                await unitOfWork.SaveChangesAsync();

                return storedFile;
            }
            catch
            {
                if (uploaded)
                {
                    await TryCleanupAsync(provider, storageKey);
                }

                throw;
            }
        }

        // =====================================
        // Download by StorageKey
        // =====================================

        public async Task<Stream> DownloadAsync(string storageKey, StorageProviderType provider, CancellationToken cancellationToken = default)
        {
            ValidateStorageKey(storageKey);

            var storageProvider = GetProvider(provider);

            var stream = await storageProvider.DownloadAsync(storageKey, cancellationToken);

            return stream ?? throw new FileNotFoundException(
                    $"Storage object '{storageKey}' was not found.");
        }

        // =====================================
        // Physical Delete Only
        //
        // Used internally when we only know
        // StorageKey + Provider
        // =====================================

        public async Task DeleteAsync(string storageKey, StorageProviderType provider, CancellationToken cancellationToken = default)
        {
            ValidateStorageKey(storageKey);

            var storageProvider = GetProvider(provider);

            await storageProvider.DeleteAsync(storageKey, cancellationToken);
        }

        // =====================================================
        // Delete StoredFile
        //
        // Physical delete + StoredFile state
        // =====================================================

        public async Task DeleteAsync(StoredFile file, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);

            // Idempotency
            if (file.IsPhysicallyDeleted)
            {
                if (!file.IsDeleted)
                {
                    file.IsDeleted = true;

                    file.DeletedAt ??= DateTime.UtcNow;

                    unitOfWork.StoredFiles.Update(file);

                    await unitOfWork.SaveChangesAsync();
                }

                return;
            }


            // -----------------------------------------
            // Physical storage
            // -----------------------------------------

            await DeleteAsync(file.StorageKey, file.Provider, cancellationToken);


            // -----------------------------------------
            // Database state
            // -----------------------------------------

            file.IsDeleted = true;

            file.IsPhysicallyDeleted = true;

            file.DeletedAt = DateTime.UtcNow;

            unitOfWork.StoredFiles.Update(file);

            await unitOfWork.SaveChangesAsync();
        }

        // =====================================
        // URL by StorageKey
        // =====================================

        public string GetFileUrl(string storageKey, StorageProviderType provider)
        {
            ValidateStorageKey(storageKey);

            var storageProvider = GetProvider(provider);

            return storageProvider.GetFileUrl(storageKey);
        }


        private async Task TryCleanupAsync(IStorageProvider provider, string storageKey)
        {
            try
            {
                await provider.DeleteAsync(storageKey, CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(cleanupException, "Failed to cleanup storage object {StorageKey}.", storageKey);
            }
        }

        private static void ValidateStorageKey(string storageKey)
        {
            if (string.IsNullOrWhiteSpace(storageKey))
            {
                throw new ArgumentException("Storage key is required.", nameof(storageKey));
            }
        }


    }
    
}
