using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IStoredFileRepository : IBaseRepository< StoredFile,Guid>
    {
        Task<IEnumerable<StoredFile>> GetDeletedFilesAsync(DateTime olderThan);
    }
}
