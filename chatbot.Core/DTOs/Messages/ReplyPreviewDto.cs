using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs.Messages
{
    public class ReplyPreviewDto
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public string SenderName { get; set; }
            = string.Empty;

        public string? Content { get; set; }

        public MessageType MessageType { get; set; }

        public bool IsDeletedForEveryone { get; set; }

        //public List<FileMetadataDto> Files { get; set; }
         //   = new();
    }
}
