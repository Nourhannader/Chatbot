using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services.Helper;

namespace chatbot.Ef.Services.Helper
{
    public sealed class TemporaryFileService : ITemporaryFileService
    {
        public async Task<string> CreateAsync(
            Stream stream,
            string extension,
            CancellationToken cancellationToken = default)
        {
            extension =
                extension.StartsWith('.')
                    ? extension
                    : $".{extension}";

            var path =
                Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid():N}{extension}");

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            await using var fileStream =
                new FileStream(
                    path,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    FileOptions.Asynchronous);

            await stream.CopyToAsync( fileStream,cancellationToken);

            return path;
        }

        public void Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // ILogger can be used here.
            }
        }
    }
}
