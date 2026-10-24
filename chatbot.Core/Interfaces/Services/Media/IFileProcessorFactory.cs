using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.Interfaces.Services.Media
{
    public interface IFileProcessorFactory
    {
        IFileProcessorService GetProcessor(MediaType mediaType);
    }
}
