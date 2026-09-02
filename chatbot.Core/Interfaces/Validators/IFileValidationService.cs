using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Validators
{
    public interface IFileValidationService
    {
        Task ValidateFile(IFormFile file);
    }
}
