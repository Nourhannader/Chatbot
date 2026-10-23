using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services.Helper;
using Microsoft.Extensions.Configuration;

namespace chatbot.Ef.Services.Helper
{
    public sealed class FfmpegService : IFfmpegService
    {
        private readonly IConfiguration _configuration;

        public FfmpegService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> RunFfprobeAsync(string arguments,CancellationToken cancellationToken = default)
        {
            var executable = _configuration["FFmpeg:FfprobePath"] ?? "ffprobe";

            return RunProcessAsync( executable, arguments,captureOutput: true,cancellationToken);
        }

        public async Task RunFfmpegAsync( string arguments,CancellationToken cancellationToken = default)
        {
            var executable = _configuration["FFmpeg:FfmpegPath"]?? "ffmpeg";

            await RunProcessAsync( executable, arguments,captureOutput: false, cancellationToken);
        }

        private static async Task<string> RunProcessAsync( string executable, string arguments,bool captureOutput, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,

                UseShellExecute = false,

                RedirectStandardOutput = true,
                RedirectStandardError = true,

                CreateNoWindow = true
            };

            using var process = new Process
                {
                    StartInfo = startInfo
                };

            if (!process.Start())
            {
                throw new InvalidOperationException( $"Could not start {executable}.");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);

            var errorTask = process.StandardError.ReadToEndAsync( cancellationToken);

            await process.WaitForExitAsync( cancellationToken);

            var output = await outputTask;

            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException( $"{executable} failed: {error}");
            }

            return captureOutput ? output : string.Empty;
        }
    }
}
