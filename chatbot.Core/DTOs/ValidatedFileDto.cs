using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class ValidatedFileDto
    {
        public string OriginalFileName { get; set; }
            = string.Empty;

        public string Extension { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public FileCategory Category { get; set; }

        public long FileSize { get; set; }
    }
}
