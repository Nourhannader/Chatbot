using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services.Helper;
using chatbot.Core.Interfaces.Services.Media;

namespace chatbot.Ef.Services.Media
{
    public sealed class VideoProcessorService : IVideoProcessorService
    {
        private readonly IFfmpegService _ffmpeg;
        private readonly ITemporaryFileService _temporaryFiles;

        public VideoProcessorService(IFfmpegService ffmpeg, ITemporaryFileService temporaryFiles)
        {
            _ffmpeg = ffmpeg;
            _temporaryFiles = temporaryFiles;
        }

        public async Task<MediaProcessingResultDto> ProcessAsync( Stream stream,string contentType,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            string? inputPath = null;
            string? thumbnailPath = null;

            try
            {
                inputPath =await _temporaryFiles.CreateAsync( stream,".mp4",cancellationToken);

                var metadata =await GetMetadataAsync(inputPath,cancellationToken);

                thumbnailPath = Path.Combine(
                        Path.GetTempPath(),
                        $"{Guid.NewGuid():N}.jpg");

                await CreateThumbnailAsync(inputPath,thumbnailPath, metadata.DurationSeconds,cancellationToken);

                var thumbnailBytes = await File.ReadAllBytesAsync(thumbnailPath, cancellationToken);

                var thumbnailStream = new MemoryStream(thumbnailBytes, writable: false);

                return new MediaProcessingResultDto
                {
                    Width = metadata.Width,

                    Height =metadata.Height,

                    DurationSeconds = metadata.DurationSeconds,

                    ThumbnailStream =thumbnailStream,

                    ThumbnailContentType ="image/jpeg"
                };
            }
            finally
            {
                if (inputPath != null)
                {
                    _temporaryFiles.Delete(inputPath);
                }

                if (thumbnailPath != null)
                {
                    _temporaryFiles.Delete(thumbnailPath);
                }
            }
        }

        private async Task<VideoMetadata> GetMetadataAsync(string path, CancellationToken cancellationToken)
        {
            var output =await _ffmpeg.RunFfprobeAsync(
                    $"-v error " +
                    $"-select_streams v:0 " +
                    $"-show_entries " +
                    $"stream=width,height:format=duration " +
                    $"-of json " +
                    $"\"{path}\"",
                    cancellationToken);

            using var document = JsonDocument.Parse(output);

            var root =document.RootElement;

            if (!root.TryGetProperty("streams",out var streams) || streams.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("No video stream was found.");
            }

            var videoStream =streams[0];

            var width =videoStream
                    .GetProperty("width")
                    .GetInt32();

            var height = videoStream
                    .GetProperty("height")
                    .GetInt32();

            if (!root.TryGetProperty( "format", out var format) ||
                !format.TryGetProperty("duration",out var durationElement))
            {
                throw new InvalidOperationException( "Unable to determine video duration.");
            }

            var durationText = durationElement.GetString();

            if (!double.TryParse(durationText,NumberStyles.Float, CultureInfo.InvariantCulture,out var duration))
            {
                throw new InvalidOperationException("Invalid video duration.");
            }

            return new VideoMetadata
            {
                Width = width,
                Height = height,
                DurationSeconds = duration
            };
        }

        private Task CreateThumbnailAsync(string inputPath,string outputPath,double duration,CancellationToken cancellationToken)
        {
            var seekPosition = Math.Min( Math.Max(duration * 0.1, 0),1);

            var seek =seekPosition.ToString( CultureInfo.InvariantCulture);

            var arguments =
                $"-y " +
                $"-ss {seek} " +
                $"-i \"{inputPath}\" " +
                $"-frames:v 1 " +
                $"-vf \"scale=300:300:force_original_aspect_ratio=decrease\" " +
                $"-q:v 3 " +
                $"\"{outputPath}\"";

            return _ffmpeg.RunFfmpegAsync(arguments, cancellationToken);
        }
        private sealed class VideoMetadata
        {
            public int Width { get; init; }

            public int Height { get; init; }

            public double DurationSeconds { get; init; }
        }
    }
}