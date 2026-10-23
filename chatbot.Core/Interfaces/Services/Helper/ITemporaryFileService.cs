using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services.Helper
{
    public interface ITemporaryFileService
    {
        Task<string> CreateAsync(Stream stream, string extension, CancellationToken cancellationToken = default);
        void Delete(string path);
    }
}
