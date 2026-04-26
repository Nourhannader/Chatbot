using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using System.IdentityModel.Tokens.Jwt;

namespace chatbot.Core.Interfaces.Services
{
    public interface IJwtService
    {
        Task<JwtSecurityToken> GenerateToken(ApplicationUser user); 
        RefreshToken GenerateRefreshToken();
    }
}
