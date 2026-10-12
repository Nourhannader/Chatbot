using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class AddMembersDto
    {
        public List<Guid> UserIds { get; set; } = new();
    }
}
