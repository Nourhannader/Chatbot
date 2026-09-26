using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class CreateGroupDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

    }
}
