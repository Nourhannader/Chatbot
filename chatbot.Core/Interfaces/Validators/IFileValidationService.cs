using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Validators
{
    public interface IFileValidationService
    {
        Task<ValidatedFileDto> ValidateFileAsync(IFormFile file,CancellationToken cancellationToken = default);
        Task<ValidatedFileDto> ValidateImageAsync(IFormFile file,CancellationToken cancellationToken = default);
        Task<ValidatedFileDto> ValidateVoiceAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
