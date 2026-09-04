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
        public async Task ValidateFile(IFormFile file,CancellationToken cancellationToken=default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid or empty file");
            if (file.Length > MaxSize)
                throw new ArgumentException("Maximum file size is 50 MB");

            var extension=Path.GetExtension(file.FileName).ToLowerInvariant();
            var mimeType = file.ContentType.ToLowerInvariant();
            //valid Extension
            var validExtension=AllowedExtensions
                .SelectMany(x=> x.Value)
                .Contains(extension,StringComparer.OrdinalIgnoreCase);
            if(!validExtension)
            {
                throw new ArgumentException($"file extenion '{extension}' is not allowed");
            }
            //valid MimeType
            var validMimeType = AllowedMimeTypes
                .SelectMany(x => x.Value)
                .Contains(mimeType, StringComparer.OrdinalIgnoreCase);
            if (!validMimeType)
            {
                throw new ArgumentException($"MiMe Type '{mimeType}' is not allowed");
            }

            ValidateExtensionAndMimeType(extension, mimeType);

            await ValidateFileSignatureAsync(file, cancellationToken);

        }
        private void ValidateExtensionAndMimeType(string extension,string mimeType)
        {
            var validPairs = new Dictionary<string, string[]>
            {
                [".jpg"] = ["image/jpeg"],
                [".jpeg"] = ["image/jpeg"],
                [".png"] = ["image/png"],
                [".gif"] = ["image/gif"],

                [".pdf"] = ["application/pdf"],

                [".docx"] =
            [
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            ],

                [".mp4"] = ["video/mp4"],

                [".mp3"] = ["audio/mpeg"],
                [".mpeg"] = ["audio/mpeg"],
                [".wav"] = ["audio/wav", "audio/x-wav"]
            };
            if(!validPairs.TryGetValue(extension,out var validMimeTypes)
                || !validMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"File extension '{extension}' and MIME type '{mimeType}' do not match or are not allowed.");
            }

        }

        private async Task ValidateFileSignatureAsync(IFormFile file,CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            var header = new byte[16];
            var bytesRead= await stream.ReadAsync(header.AsMemory(0,header.Length), cancellationToken);

            if (bytesRead == 0)
            {
                throw new ArgumentException("Invalid file content.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isValid = extension switch
            {
                ".jpg" or ".jpeg" =>
               IsMatch(header, 0xFF, 0xD8, 0xFF),

                ".png" =>
                    IsMatch(
                        header,
                        0x89, 0x50, 0x4E, 0x47,
                        0x0D, 0x0A, 0x1A, 0x0A),

                ".gif" =>
                    IsGif(header),

                ".pdf" =>
                    IsMatch(header, 0x25, 0x50, 0x44, 0x46),

                ".mp3" =>
                    IsMp3(header),

                ".wav" =>
                    IsWav(header),

                _ => true
            };
            if (!isValid)
            {
                throw new ArgumentException("File signature is invalid.");
            }

        }
        private static bool IsMatch( byte[] header,params byte[] signature)
        {
            if (header.Length < signature.Length)
                return false;

            return header
                .Take(signature.Length)
                .SequenceEqual(signature);
        }
        private static bool IsGif(byte[] header)
        {
            return IsMatch(
                       header,
                       (byte)'G',
                       (byte)'I',
                       (byte)'F',
                       (byte)'8',
                       (byte)'7',
                       (byte)'a')
                   ||
                   IsMatch(
                       header,
                       (byte)'G',
                       (byte)'I',
                       (byte)'F',
                       (byte)'8',
                       (byte)'9',
                       (byte)'a');
        }


        private static bool IsWav(byte[] header)
        {
            return header.Length >= 12 &&
                   header[0] == (byte)'R' &&
                   header[1] == (byte)'I' &&
                   header[2] == (byte)'F' &&
                   header[3] == (byte)'F' &&
                   header[8] == (byte)'W' &&
                   header[9] == (byte)'A' &&
                   header[10] == (byte)'V' &&
                   header[11] == (byte)'E';
        }


        private static bool IsMp3(byte[] header)
        {
            // ID3 header
            if (header.Length >= 3 &&
                header[0] == (byte)'I' &&
                header[1] == (byte)'D' &&
                header[2] == (byte)'3')
            {
                return true;
            }


            // MPEG frame sync
            return header.Length >= 2 &&
                   header[0] == 0xFF &&
                   (header[1] & 0xE0) == 0xE0;
        }
    }
}
