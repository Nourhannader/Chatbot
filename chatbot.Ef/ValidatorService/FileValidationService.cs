using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Validators;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.ValidatorService
{
    public class FileValidationService : IFileValidationService
    {
        private const long MaxFileSize = 50 * 1024 * 1024;//50MB
        private const long MaxImageSize = 5 * 1024 * 1024;//5MB
        private const long MaxVoiceSize = 20 * 1024 * 1024;//20MB
        private sealed record FileRule(MediaType MediaType, string[] MimeTypes);

        private static readonly IReadOnlyDictionary<string, FileRule>
        Rules = new Dictionary<string, FileRule>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = new(MediaType.Image,["image/jpeg"]),

            [".jpeg"] = new(MediaType.Image,["image/jpeg"]),

            [".png"] = new(MediaType.Image,["image/png"]),

            [".gif"] = new(MediaType.Image,["image/gif"]),

            [".pdf"] = new(MediaType.Document,["application/pdf"]),

            [".docx"] = new(MediaType.Document, ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"]),

            [".mp4"] = new(MediaType.Video,["video/mp4"]),

            [".mp3"] = new(MediaType.Audio, ["audio/mpeg","audio/mp3"]),

            [".mpeg"] = new(MediaType.Audio,["audio/mpeg"]),

            [".wav"] = new( MediaType.Audio,[ "audio/wav","audio/x-wav","audio/wave" ])
        };
        public async Task<ValidatedFileDto> ValidateFileAsync(IFormFile file,CancellationToken cancellationToken=default)
        {
            return await ValidateAsync(file, expectedMediaType: null, MaxFileSize, cancellationToken);
        }
        public async Task<ValidatedFileDto> ValidateImageAsync(IFormFile file,CancellationToken cancellationToken= default)
        {
            return await ValidateAsync(file, MediaType.Image , MaxImageSize, cancellationToken);
        }
        public async Task<ValidatedFileDto> ValidateVoiceAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await ValidateAsync(file, MediaType.Audio, MaxVoiceSize , cancellationToken);
        }

        private static async Task<ValidatedFileDto> ValidateAsync(IFormFile file, MediaType? expectedMediaType,
        long maximumSize, CancellationToken cancellationToken)
        {
            ValidateBasicFile(file, maximumSize);

            var safeFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(safeFileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ValidationException("File extension is required.");
            }

            if (!Rules.TryGetValue(extension, out var rule))
            {
                throw new ValidationException($"File extension '{extension}' is not allowed.");
            }

            if (expectedMediaType.HasValue && rule.MediaType != expectedMediaType.Value)
            {
                throw new ValidationException($"The selected file must be a " +
                    $"{expectedMediaType.Value.ToString().ToLowerInvariant()} file.");
            }

            var mimeType = NormalizeMimeType(file.ContentType);

            if (!rule.MimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException($"File extension '{extension}' does not match " +
                    $"MIME type '{mimeType}'.");
            }

            await ValidateFileSignatureAsync(file, extension, cancellationToken);

            return new ValidatedFileDto
            {
                OriginalFileName = safeFileName,
                Extension = extension,
                ContentType = mimeType,
                FileSize = file.Length,
                MediaType = rule.MediaType
            };
        }
        private static void ValidateBasicFile(IFormFile file, long maximumSize)
        {
            if (file == null)
            {
                throw new ValidationException("File is required.");
            }

            if (file.Length <= 0)
            {
                throw new ValidationException("File cannot be empty.");
            }

            if (file.Length > maximumSize)
            {
                var maximumMb = maximumSize / 1024 / 1024;

                throw new ValidationException($"Maximum file size is {maximumMb} MB.");
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                throw new ValidationException("File name is required.");
            }
        }

        private static string NormalizeMimeType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ValidationException("File MIME type is required.");
            }

            return contentType.Split(';', 2)[0].Trim().ToLowerInvariant();
        }

        private static async Task ValidateFileSignatureAsync(IFormFile file, string extension, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            var header = new byte[32];
            var bytesRead= await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);


            if (bytesRead <= 0)
            {
                throw new ValidationException("Invalid file content.");
            }

            if (extension == ".docx")
            {
                var validDocx = IsZip(header, bytesRead) &&
                    await IsDocxAsync(file, cancellationToken);

                if (!validDocx)
                {
                    throw new ValidationException("File content is not a valid DOCX document.");
                }

                return;
            }

            var isValid = extension switch
            {
                ".jpg" or ".jpeg" =>
               IsMatch(header, bytesRead ,0xFF, 0xD8, 0xFF),

                ".png" =>
                    IsMatch(
                        header,
                        bytesRead,
                        0x89, 0x50, 0x4E, 0x47,
                        0x0D, 0x0A, 0x1A, 0x0A),

                ".gif" =>
                    IsGif(header,bytesRead),

                ".pdf" =>
                    IsMatch(header,bytesRead ,0x25, 0x50, 0x44, 0x46),

                ".mp4" =>
                    IsMp4(header, bytesRead),

                ".mp3" =>
                    IsMp3(header, bytesRead),

                ".mpeg" =>
                    IsMpegAudio(header, bytesRead),

                ".wav" =>
                    IsWav(header, bytesRead),

                _ => false
            };
            if (!isValid)
            {
                throw new ValidationException($"File content does not match extension '{extension}'.");
            }

        }

        private static async Task<bool> IsDocxAsync(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                await using var stream = file.OpenReadStream();

                using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);

                cancellationToken.ThrowIfCancellationRequested();

                var hasContentTypes = archive.GetEntry("[Content_Types].xml") != null;

                var hasDocument = archive.GetEntry("word/document.xml") != null;

                return hasContentTypes && hasDocument;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }
        private static bool IsMatch( byte[] header, int bytesRead, params byte[] signature)
        {
            if (bytesRead < signature.Length)
                return false;

            for (var i = 0; i < signature.Length; i++)
            {
                if (header[i] != signature[i])
                    return false;
            }

            return true;
        }
        private static bool IsGif(byte[] header, int bytesRead)
        {
            if (bytesRead < 6)
                return false;

            return IsMatch(
                       header,
                       bytesRead,
                       (byte)'G',
                       (byte)'I',
                       (byte)'F',
                       (byte)'8',
                       (byte)'7',
                       (byte)'a')
                   ||
                   IsMatch(
                       header,
                       bytesRead,
                       (byte)'G',
                       (byte)'I',
                       (byte)'F',
                       (byte)'8',
                       (byte)'9',
                       (byte)'a');
        }
        private static bool IsZip(byte[] header, int bytesRead)
        {
            // DOCX is an OpenXML ZIP container.
            return
                IsMatch(
                    header,
                    bytesRead,
                    0x50, 0x4B, 0x03, 0x04)
                ||
                IsMatch(
                    header,
                    bytesRead,
                    0x50, 0x4B, 0x05, 0x06)
                ||
                IsMatch(
                    header,
                    bytesRead,
                    0x50, 0x4B, 0x07, 0x08);
        }
        private static bool IsMp4(byte[] header, int bytesRead)
        {
            if (bytesRead < 12)
                return false;

            // ISO Base Media File Format:
            // bytes 4-7 should normally be "ftyp".
            return
                header[4] == (byte)'f' &&
                header[5] == (byte)'t' &&
                header[6] == (byte)'y' &&
                header[7] == (byte)'p';
        }
        private static bool IsWav(byte[] header, int bytesRead)
        {
            if (bytesRead < 12)
                return false;
            return
            header[0] == (byte)'R' &&
            header[1] == (byte)'I' &&
            header[2] == (byte)'F' &&
            header[3] == (byte)'F' &&

            header[8] == (byte)'W' &&
            header[9] == (byte)'A' &&
            header[10] == (byte)'V' &&
            header[11] == (byte)'E';
        }
        private static bool IsMp3(byte[] header, int bytesRead)
        {
            if (bytesRead >= 3 &&
            header[0] == (byte)'I' &&
            header[1] == (byte)'D' &&
            header[2] == (byte)'3')
            {
                return true;
            }

            return IsMpegAudio(header, bytesRead);
        }
        private static bool IsMpegAudio(byte[] header, int bytesRead)
        {
            return
                bytesRead >= 2 &&
                header[0] == 0xFF &&
                (header[1] & 0xE0) == 0xE0;
        }


    }
}
