using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class MediaDownloadResultDto
    {
        public Stream Stream { get; set; }

         public  string ContentType { get; set; }

         public string FileName { get; set; }

        public long FileSize { get; set; }
    }
}
