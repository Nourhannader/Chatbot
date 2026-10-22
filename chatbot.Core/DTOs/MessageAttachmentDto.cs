using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Messages;

namespace chatbot.Core.DTOs
{
    public class MessageAttachmentDto
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string? Caption { get; set; }

        public FileMetadataDto File { get; set; }
            = null!;
    }
}
