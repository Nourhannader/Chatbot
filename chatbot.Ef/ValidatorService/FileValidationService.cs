using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Validators;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.ValidatorService
{
    public class FileValidationService : IFileValidationService
    {
        private readonly Dictionary<string, string[]> AllowedExtensions = new()
        {
            ["image"] = [".jpg", ".jpeg", ".png", ".gif"],
            ["document"] = [".pdf", ".docx"],
            ["video"] = [".mp4"],
            ["audio"] = [".mp3", ".wav", ".mpeg"]
        };
        private readonly Dictionary<string, string[]> AllowedMimeTypes = new()
        {
            ["image"] =
        [
            "image/jpeg",
            "image/png",
            "image/gif"
        ],

            ["document"] =
        [
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        ],

            ["video"] =
        [
            "video/mp4"
        ],

            ["audio"] =
        [
            "audio/mpeg",
            "audio/wav",
            "audio/x-wav"
        ]
        };

        private const long MaxSize = 50 * 1024 * 1024;
        public async Task ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid or empty file");
            if (file.Length > MaxSize)
                throw new Exception("Maximum file size is 50 MB");

            var extension=Path.GetExtension(file.FileName).ToLowerInvariant();
            var mimeType = file.ContentType.ToLowerInvariant();
            //valid Extension
            var validExtension=AllowedExtensions.SelectMany(x=> x.Value).Contains(extension,StringComparer.OrdinalIgnoreCase);
            if(!validExtension)
            {
                throw new Exception($"file extenion '{extension}' is not allowed");
            }
            //valid MimeType
            var validMimeType = AllowedMimeTypes.SelectMany(x => x.Value).Contains(mimeType, StringComparer.OrdinalIgnoreCase);
            if (!validMimeType)
            {
                throw new Exception($"MiMe Type '{mimeType}' is not allowed");
            }

            await Task.CompletedTask;

        }
    }
}
