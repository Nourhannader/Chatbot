using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace chatbot.Ef.Services
{
    public class ImageProcessorService : IFileProcessorService
    {
        public async Task<string> createThumbnailAsync(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            var thumbnailPath = Path.Combine(directory, $"{fileName}_thumb{extension}");
            
            using var image =await Image.LoadAsync(path);
            image.Mutate(
                x => x.Resize(new ResizeOptions
                {
                    Size=new Size(300,300),
                    Mode = ResizeMode.Max
                }));
            await image.SaveAsync(thumbnailPath);
            return thumbnailPath;
            
        }
    }
}
