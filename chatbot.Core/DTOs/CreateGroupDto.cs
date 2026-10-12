using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs
{
    public class CreateGroupDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

        public List<Guid> MemberIds { get; set; } = new();

    }
}
