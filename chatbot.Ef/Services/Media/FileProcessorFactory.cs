using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.Media;

namespace chatbot.Ef.Services.Media
{
    public class FileProcessorFactory(IImageProcessorService imageProcessor,IAudioProcessorService audioProcessor,IVideoProcessorService videoProcessor) :IFileProcessorFactory
    {
        public IFileProcessorService GetProcessor(MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.Image =>
                    imageProcessor,

                 MediaType.Video =>
                    videoProcessor,

                MediaType.Audio =>
                    audioProcessor,

                _ => throw new NotSupportedException(
                    $"Processing category '{mediaType}' is not supported.")
            };
        }


    }
}
