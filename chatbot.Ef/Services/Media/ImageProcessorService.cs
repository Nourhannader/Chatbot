using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace chatbot.Ef.Services
{
    public class ImageProcessorService : IImageProcessorService
    {
        private const int ThumbnailWidth = 300;
        private const int ThumbnailHeight = 300;
        public async Task<MediaProcessingResultDto> ProcessAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (stream.CanSeek)
                stream.Position = 0;

            using var image = await Image.LoadAsync(stream, cancellationToken);

            var originalWidth = image.Width;

            var originalHeight = image.Height;

            image.Mutate(context => 
              context.Resize(new ResizeOptions
              {
                  Size = new Size(ThumbnailWidth, ThumbnailHeight),
                  Mode = ResizeMode.Max
              })
            );
            var thumbnailStream = new MemoryStream();

            try
            {
                await image.SaveAsJpegAsync(thumbnailStream, cancellationToken);
                thumbnailStream.Position = 0;

                return new MediaProcessingResultDto
                {
                    Width = originalWidth,
                    Height = originalHeight,
                    ThumbnailStream = thumbnailStream,
                    ThumbnailContentType = "image/jpeg"
                };
            }
            catch
            {
                await thumbnailStream.DisposeAsync();
                throw;
            }
        }
    }
}
