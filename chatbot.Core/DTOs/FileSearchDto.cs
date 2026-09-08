using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class FileSearchDto
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public string FileUrl { get; set; }
    }
}
