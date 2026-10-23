using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services.Helper;
using chatbot.Core.Interfaces.Services.Media;

namespace chatbot.Ef.Services.Media
{
    public sealed class AudioProcessorService : IAudioProcessorService
    {
        private const int WaveformPoints = 100;

        private readonly IFfmpegService _ffmpeg;
        private readonly ITemporaryFileService _temporaryFiles;

        public AudioProcessorService(IFfmpegService ffmpeg, ITemporaryFileService temporaryFiles)
        {
            _ffmpeg = ffmpeg;
            _temporaryFiles = temporaryFiles;
        }

        public async Task<MediaProcessingResultDto> ProcessAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            var extension = GetExtension(contentType);

            string? inputPath = null;
            string? pcmPath = null;

            try
            {
                inputPath =
                    await _temporaryFiles.CreateAsync(
                        stream,
                        extension,
                        cancellationToken);

                // =========================
                // Duration
                // =========================

                var duration = await GetDurationAsync(inputPath, cancellationToken);

                // =========================
                // PCM for waveform
                // =========================

                pcmPath =
                    Path.Combine(
                        Path.GetTempPath(),
                        $"{Guid.NewGuid():N}.pcm");

                await GeneratePcmAsync(inputPath, pcmPath, cancellationToken);

                var waveform =
                    await GenerateWaveformAsync(pcmPath, WaveformPoints, cancellationToken);

                return new MediaProcessingResultDto
                {
                    DurationSeconds = duration,
                    Waveform = waveform
                };
            }
            finally
            {
                if (inputPath != null)
                {
                    _temporaryFiles.Delete(
                        inputPath);
                }

                if (pcmPath != null)
                {
                    _temporaryFiles.Delete(
                        pcmPath);
                }
            }
        }


        private async Task<double> GetDurationAsync(string path,CancellationToken cancellationToken)
        {
            var output =
                await _ffmpeg.RunFfprobeAsync(
                    $"-v error " +
                    $"-show_entries format=duration " +
                    $"-of default=noprint_wrappers=1:nokey=1 " +
                    $"\"{path}\"",
                    cancellationToken);

            if (!double.TryParse(
                    output.Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var duration))
            {
                throw new InvalidOperationException(
                    "Unable to determine audio duration.");
            }

            return duration;
        }

        private Task GeneratePcmAsync( string inputPath,string outputPath,CancellationToken cancellationToken)
        {
            var arguments =
                $"-y " +
                $"-i \"{inputPath}\" " +
                $"-vn " +
                $"-ac 1 " +
                $"-ar 8000 " +
                $"-f s16le " +
                $"\"{outputPath}\"";

            return _ffmpeg.RunFfmpegAsync(
                arguments,
                cancellationToken);
        }

        private static async Task<List<double>> GenerateWaveformAsync(string pcmPath, int points, CancellationToken cancellationToken)
        {
            var bytes = await File.ReadAllBytesAsync( pcmPath,cancellationToken);

            if (bytes.Length < 2)
                return [];

            var sampleCount = bytes.Length / 2;

            if (sampleCount == 0)
                return [];

            var bucketCount =Math.Min(points, sampleCount);

            var samplesPerBucket = Math.Max(1,sampleCount / bucketCount);

            var waveform =new List<double>(bucketCount);

            for (var bucket = 0;bucket < bucketCount; bucket++)
            {
                var startSample =bucket * samplesPerBucket;

                var endSample = bucket == bucketCount - 1
                        ? sampleCount : Math.Min(sampleCount, startSample + samplesPerBucket);

                var peak = 0;

                for (var sampleIndex = startSample; sampleIndex < endSample; sampleIndex++)
                {
                    var byteIndex = sampleIndex * 2;

                    var sample =BitConverter.ToInt16(bytes, byteIndex);

                    // -32768 needs special handling.
                    var amplitude =sample == short.MinValue
                            ? 32768 : Math.Abs((int)sample);

                    if (amplitude > peak)
                    {
                        peak = amplitude;
                    }
                }

                var normalized =peak / 32768.0;

                waveform.Add( Math.Round( normalized,3));
            }

            return waveform;
        }

        private static string GetExtension(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "audio/mpeg" =>
                    ".mp3",

                "audio/mp3" =>
                    ".mp3",

                "audio/wav" =>
                    ".wav",

                "audio/x-wav" =>
                    ".wav",

                "audio/wave" =>
                    ".wav",

                _ =>
                    ".audio"
            };
        }
    }
}