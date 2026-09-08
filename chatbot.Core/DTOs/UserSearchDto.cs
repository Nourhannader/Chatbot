using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs
{
    public class UserSearchDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
