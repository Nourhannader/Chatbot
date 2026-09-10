using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace chatbot.Ef.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings settings;
        public JwtService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> settings)
        {
            this._userManager = userManager;
            this.settings = settings.Value;
        }
        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);

        }

        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

                 new Claim( ClaimTypes.Email,user.Email ?? string.Empty),

                new Claim(ClaimTypes.Name,user.UserName ?? string.Empty),
                new Claim("uid", user.Id.ToString()) // Convert Guid to string
            }
            .Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
