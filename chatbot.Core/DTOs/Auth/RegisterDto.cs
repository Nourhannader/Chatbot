using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ServiceStack.DataAnnotations;

namespace chatbot.Core.DTOs.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }

        public string? Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [Unique]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("password")]
        [DataType(DataType.Password)]

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
