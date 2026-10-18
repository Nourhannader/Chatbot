using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class StickerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public string Url { get; set; }
            = string.Empty;
    }
}
