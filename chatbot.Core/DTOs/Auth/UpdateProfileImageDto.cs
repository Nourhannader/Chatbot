using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.DTOs.Auth
{
    public sealed class UpdateProfileImageDto
    {
        public IFormFile Image { get; set; } = null!;
    }
}
