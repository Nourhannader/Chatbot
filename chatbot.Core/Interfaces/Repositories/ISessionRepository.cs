using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        Task<DeviceSession?> GetById(Guid sessionId);

        Task<List<DeviceSession>> GetAllActiveSession();
        Task<List<DeviceSession>> GetAllActiveSession(Guid userId);
    }
}
