using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IRolePermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, Guid conversationId, string permission);
    }
}
