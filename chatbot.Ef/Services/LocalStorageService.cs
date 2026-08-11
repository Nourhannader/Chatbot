using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace chatbot.Ef.Services
{
    public class LocalStorageService(IWebHostEnvironment environment) : IStorageService 
    {
        public Task DeleteAsync(string fileUrl)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> DownloadAsync(string fileUrl)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fileUrl)
        {
            throw new NotImplementedException();
        }

        public Task<UploadResultDto> UploadAsync(IFormFile file, string folder)
        {
            throw new NotImplementedException();
        }
    }
}
