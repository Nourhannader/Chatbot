using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class FileUploadResultDto
    {
        public string StoredFileName { get; set; }
            = string.Empty;

        public string StorageKey { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long FileSize { get; set; }

        public StorageProviderType Provider { get; set; }
    }
}
