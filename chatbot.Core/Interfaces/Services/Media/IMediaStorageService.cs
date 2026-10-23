using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services.Media
{
    public interface IMediaStorageService
    {
        Task<StoredFile> UploadAsync(IFormFile file, ValidatedFileDto validatedFile,
        Guid uploadedById, string folder, CancellationToken cancellationToken = default);
        Task DeleteAsync(StoredFile file, CancellationToken cancellationToken = default);
    }
}
