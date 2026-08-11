using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace chatbot.Core.Interfaces.Services
{
    public interface IStorageService
    {
        Task<UploadResultDto> UploadAsync(IFormFile file,string folder);

        Task DeleteAsync(string fileUrl);

        Task<Stream> DownloadAsync(string fileUrl);

        bool Exists(string fileUrl);
    }
}
