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
        private readonly List<string> allowed = [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".mp4", ".mp3", ".wav"];
        private const long MaxSize = 20 * 1024 * 1024;
        public void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid file");
            if (file.Length > MaxSize)
                throw new Exception("File is too Large");

            var extension=Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(extension))
            {
                throw new Exception("Unsupported file.");
            }

        }
    }
}
