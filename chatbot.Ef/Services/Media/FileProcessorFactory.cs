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
        public IFileProcessorService GetProcessor(FileCategory category)
        {
            return category switch
            {
                FileCategory.Image =>
                    imageProcessor,

                FileCategory.Video =>
                    videoProcessor,

                FileCategory.Audio =>
                    audioProcessor,

                _ => throw new NotSupportedException(
                    $"Processing category '{category}' is not supported.")
            };
        }


    }
}
