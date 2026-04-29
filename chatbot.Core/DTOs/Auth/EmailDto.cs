using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Auth
{
    public class EmailDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
