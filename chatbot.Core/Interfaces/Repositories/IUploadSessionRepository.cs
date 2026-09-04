using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IUploadSessionRepository:IBaseRepository<UploadSession,Guid>
    {
        Task<List<UploadSession>> GetExpiredSessionsAsync(DateTime expiredBefore);
        void Remove(UploadSession uploadSession);
    }
}
