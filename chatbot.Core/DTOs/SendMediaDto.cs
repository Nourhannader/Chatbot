using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs
{
    public class SendMediaDto
    {
        public Guid ConversationId { get; set; }

        public List<IFormFile> Files { get; set; } = [];

        public string? Caption { get; set; }
    }
}
