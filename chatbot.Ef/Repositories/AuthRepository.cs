using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.AspNetCore.Identity;

namespace chatbot.Ef.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AuthRepository(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
           this._userManager = userManager;
            this._context = context;
        }
        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password) =>
            _userManager.CheckPasswordAsync(user, password);


        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password) =>
            _userManager.CreateAsync(user, password);
        

        public Task<ApplicationUser> GetByEmailAsync(string email) =>
            _userManager.FindByEmailAsync(email);

        public Task<ApplicationUser> GetByNameAsync(string username) =>
            _userManager.FindByNameAsync(username);
        

        public Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task SaveRefreshTokenAsync(RefreshToken token)
        {
            throw new NotImplementedException();
        }
    }
}
