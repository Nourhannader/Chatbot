using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using FluentValidation;

namespace chatbot.Core.Validators.Messages
{
    public class SendFileDtoValidator:AbstractValidator<SendMessageDto>
    {
        private static readonly string[] AllowedExtensions =
        {
          ".jpg",
          ".jpeg",
          ".png",
          ".gif",
          ".pdf",
          ".docx",
          ".mp4",
          ".mp3",
          ".wav"
        };

        private const long MaxFileSize =
            20 * 1024 * 1024;
        public SendFileDtoValidator()
        {
            RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required.");

            RuleFor(x => x.File)
                .Must(file =>
                    file == null ||
                    file.Length <= MaxFileSize)
                .WithMessage("Maximum file size is 20 MB.");

            RuleFor(x => x.File)
                .Must(file =>
                {
                    if (file == null)
                        return true;

                    var extension =
                        Path.GetExtension(
                            file.FileName)
                        .ToLowerInvariant();

                    return AllowedExtensions
                        .Contains(extension);
                })
                .WithMessage(
                    "Unsupported file type.");
        }
    }
}
