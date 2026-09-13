using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? GroupPictureUrl { get; set; }

        public Guid CreatedById { get; set; }

        public List<GroupMemberDto> Members { get; set; } = [];
    }
}
