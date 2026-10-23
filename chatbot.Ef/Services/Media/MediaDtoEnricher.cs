using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;
using chatbot.Core.Models;

namespace chatbot.Ef.Services.Media
{
    public class MediaDtoEnricher : IMediaDtoEnricher
    {
        private readonly IStorageService _storageService;

        public MediaDtoEnricher(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task EnrichAsync( Message message, MessageDto dto, CancellationToken cancellationToken = default)
        {
            for (var i = 0; i < message.Attachments.Count;i++)
            {
                var attachment = message.Attachments
                        .OrderBy(x => x.Order)
                        .ElementAt(i);

                var file = attachment.StoredFile;

                var dtoAttachment = dto.attachments[i];

                dtoAttachment.File.FileUrl =
                    await _storageService.GetFileUrlAsync(file.Id,cancellationToken)
                    ?? string.Empty;

                if (file.ThumbnailFileId.HasValue)
                {
                    dtoAttachment.File.ThumbnailUrl = await _storageService.GetFileUrlAsync(file.ThumbnailFileId.Value,cancellationToken);
                }
            }

            if (message.VoiceNote != null &&
                dto.VoiceNote != null)
            {
                dto.VoiceNote.FileUrl =await _storageService
                        .GetFileUrlAsync(message.VoiceNote.FileId,cancellationToken)
                    ?? string.Empty;

                dto.VoiceNote.Waveform = DeserializeWaveform(message.VoiceNote.Waveform);
            }
        }

        private static List<double> DeserializeWaveform(string waveform)
        {
            if (string.IsNullOrWhiteSpace(waveform))
                return new();

            try
            {
                return JsonSerializer.Deserialize<List<double>>(
                           waveform)
                       ?? new();
            }
            catch (JsonException)
            {
                return new();
            }
        }
    }
}
