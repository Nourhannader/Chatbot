using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;

namespace chatbot.Core.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenResultDto>RefreshAsync(string refreshToken);

        Task RevokeAsync(Guid sessionId);

        Task RevokeAllAsync(Guid userId);
    }
}
