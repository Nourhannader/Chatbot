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
        private string GeneratePath( string folder,string fileName)
        {
            var extension =
                Path.GetExtension(fileName)
                    .ToLowerInvariant();

            var now = DateTime.UtcNow;

            var storedName =
                $"{Guid.NewGuid()}{extension}";

            return Path.Combine(
                folder,
                now.Year.ToString(),
                now.Month.ToString("00"),
                now.Day.ToString("00"),
                storedName
            ).Replace("\\", "/");
        }
        private IStorageProvider GetProvider(StorageProviderType providerType)
        {
            return providers.First(x =>
                x.ProviderType == providerType);
        }
        public async Task DeleteAsync(Guid fileId)
        {

            var file = await unitOfWork.StoredFiles.GetByIdAsync(fileId);
            if(file == null)
                throw new Exception("File not found");
            if(file.IsDeleted)
                return;
            file.IsDeleted = true;
            file.DeletedAt = DateTime.UtcNow;
             unitOfWork.StoredFiles.Update(file);
        }

        public async Task<Stream> DownloadAsync(string fileUrl)
        {
            var path = Path.Combine(environment.WebRootPath, fileUrl.TrimStart('/'));
            return await Task.FromResult(new FileStream(path, FileMode.Open, FileAccess.Read));
        }

        public async Task<UploadResultDto> UploadAsync(IFormFile file, string folder, string uploadedBy, StorageProviderType providerType = StorageProviderType.Local)
        {
            //validation
            await validators.ValidateFile(file);
            //select provider
            var provider=GetProvider(providerType);
            //generate unique path
            var relativePath = GeneratePath(folder, file.FileName);

            //upload physical file
            await using var stream=file.OpenReadStream();
            await provider.UploadAsync(stream,relativePath, file.ContentType);
            //generate thumbnail if image
            string? thumbnailPath = null;
            if (file.ContentType.StartsWith("image/"))
            {
                thumbnailPath =
                    await processor.createThumbnailAsync(
                        relativePath);
            }
            //save file metadata to database
            var storedFile = new StoredFile
            {
                Id = Guid.NewGuid(),
                OriginalName = file.FileName,
                StoredName = Path.GetFileName(relativePath),
                ContentType = file.ContentType,
                Size = file.Length,
                Path = relativePath,
                StorageProvider = providerType,
                UploadedByUserId = Guid.Parse(uploadedBy),
                ThumbnailPath = thumbnailPath
            };
            await unitOfWork.StoredFiles.AddAsync(storedFile);

            return new UploadResultDto
            {
                Success = true,

                FileId =
                    storedFile.Id,

                FileUrl =
                    provider.GetFileUrl(
                        storedFile.Path),

                ThumbnailUrl =
                    thumbnailPath != null
                        ? provider.GetFileUrl(
                            thumbnailPath)
                        : string.Empty,

                ContentType =
                    storedFile.ContentType,

                Size =
                    storedFile.Size
            };
        }

        public Task<string?> GetFileUrlAsync(Guid fileId)
        {
            throw new NotImplementedException();
        }
    }
    
}
