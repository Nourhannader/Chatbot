using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace chatbot.Ef.Services
{
    public class LocalStorageService(IWebHostEnvironment environment) : IStorageService
    {
       
        public async Task DeleteAsync(string fileUrl)
        {

            var path = Path.Combine(environment.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            await Task.CompletedTask;
        }

        public async Task<Stream> DownloadAsync(string fileUrl)
        {
            var path = Path.Combine(environment.WebRootPath, fileUrl.TrimStart('/'));
            return await Task.FromResult(new FileStream(path, FileMode.Open, FileAccess.Read));
        }

        public bool Exists(string fileUrl)
        {
            var path = Path.Combine(environment.WebRootPath, fileUrl.TrimStart('/'));
            return File.Exists(path);
        }

        public async Task<UploadResultDto> UploadAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads", folder); 
            if (!Directory.Exists(uploadsFolder)) 
                Directory.CreateDirectory(uploadsFolder); 
            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(uploadsFolder, fileName); 
            
            using var stream = new FileStream(path, FileMode.Create); 
            await file.CopyToAsync(stream); 
            return new UploadResultDto 
            { 
                Success = true,
                FileName = fileName, 
                FileSize = file.Length, 
                ContentType = file.ContentType, 
                FileUrl = $"/uploads/{folder}/{fileName}" };
        }
    }
}
