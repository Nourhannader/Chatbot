using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IStoredFileRepository
    {
        Task AddAsync(StoredFile file);

        Task AddRangeAsync(IEnumerable<StoredFile> files);

        Task<StoredFile?> GetByIdAsync(Guid id);

        Task<List<StoredFile>> GetByMessageIdAsync( Guid messageId);

        Task<List<StoredFile>> GetFilesForCleanupAsync(DateTime olderThan);

        Task UpdateAsync(StoredFile file);

        Task DeleteAsync(StoredFile file);
    }
}
