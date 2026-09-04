using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class StartUploadDto
    {
        public string FileName { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long TotalSize { get; set; }

        public int TotalChunks { get; set; }
    }
}
