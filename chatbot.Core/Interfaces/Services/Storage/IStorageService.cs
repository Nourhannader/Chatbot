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
        // =====================================
        // Upload
        // =====================================

        Task<StoredFile> UploadAsync(IFormFile file, ValidatedFileDto validatedFile, FileCategory category,
                     Guid ownerId, Guid? uploadedById,  CancellationToken cancellationToken = default);

        Task<StoredFile> UploadGeneratedAsync(Stream stream, string fileName, string contentType, FileCategory category,
            MediaType mediaType, Guid? uploadedById,  CancellationToken cancellationToken = default);


        // =====================================
        // Download
        // =====================================

        Task<Stream> DownloadAsync(string storageKey, StorageProviderType provider, CancellationToken cancellationToken = default);

        // ==============================
        // Physical storage delete only
        // ==============================

        Task DeleteAsync(string storageKey, StorageProviderType provider, CancellationToken cancellationToken = default);


        // ==============================
        // StoredFile delete
        // Physical + database state
        // ==============================

        Task DeleteAsync(StoredFile file, CancellationToken cancellationToken = default);

        string GetFileUrl(string storageKey, StorageProviderType provider);


       
    }
}
