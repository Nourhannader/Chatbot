using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class UploadResultDto
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; }= string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
