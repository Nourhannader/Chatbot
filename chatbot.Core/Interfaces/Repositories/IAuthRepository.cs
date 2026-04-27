using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetByNameAsync(string username);
        Task<ApplicationUser?> GetByToken(string token);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        Task SaveRefreshTokenAsync(ApplicationUser user);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
    }
}
