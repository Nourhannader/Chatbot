using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace chatbot.Ef.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtSettings jwt;
        private readonly IJwtService _jwtService;
        public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwt,IJwtService jwtService)
        {
            this._authRepository = authRepository;
            this.jwt = jwt.Value;
            this._jwtService = jwtService;
        }

        public async Task<AuthResponseDto> GetTokenAsync(LoginDto model)
        {
            var authResponse = new AuthResponseDto();
            var user = await _authRepository.GetByEmailAsync(model.Email);
            if (user is null || !await _authRepository.CheckPasswordAsync(user, model.Password))
            {
                authResponse.Message = "Invalid email or password!";
                return authResponse;
            }
            var token = await _jwtService.GenerateToken(user);
            authResponse.IsAuthenticated = true;
            authResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authResponse.ExpiresOn = token.ValidTo;
            authResponse.Username = user.UserName;
            authResponse.Email = user.Email;
            authResponse.ImageProfileUrl = user.ImageProfileUrl;
            authResponse.PhoneNumber = user.PhoneNumber;

            if(user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken=user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authResponse.RefreshToken = activeRefreshToken.Token;
                authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = _jwtService.GenerateRefreshToken();
                authResponse.RefreshToken = refreshToken.Token;
                authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _authRepository.SaveRefreshTokenAsync(user);

            }


            return authResponse;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            var authModelDto = new AuthResponseDto();
            var user=await _authRepository.GetByToken(token);
            if (user is null)
            {
                authModelDto.Message = "Invalid token!";
                return authModelDto;
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                authModelDto.Message = "Inactive token!";
                return authModelDto;
            }
            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _authRepository.SaveRefreshTokenAsync(user);
            var jwtToken = await _jwtService.GenerateToken(user);
            authModelDto.IsAuthenticated = true;
            authModelDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModelDto.ExpiresOn = jwtToken.ValidTo;
            authModelDto.Username = user.UserName;
            authModelDto.Email = user.Email;
            authModelDto.ImageProfileUrl = user.ImageProfileUrl;
            authModelDto.PhoneNumber = user.PhoneNumber;
            authModelDto.RefreshToken = newRefreshToken.Token;
            authModelDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModelDto;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            if(await _authRepository.GetByEmailAsync(model.Email) is not null)
                return new AuthResponseDto { Message = "Email is already registered!" };
            if(await _authRepository.GetByNameAsync(model.UserName) is not null)
                return new AuthResponseDto { Message = "Username is already registered!" };
            var imageUrl = string.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0) {
                imageUrl = await GetImageUrl(model);
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone,
                
                ImageProfileUrl = imageUrl

            };
            var result = await _authRepository.CreateUserAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthResponseDto { Message = errors };
            }
            var token = await _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo,
                Username = user.UserName,
                Email = user.Email,
                ImageProfileUrl = user.ImageProfileUrl,
                PhoneNumber = user.PhoneNumber

            };

        }

        private async    Task<string> GetImageUrl(RegisterDto dto)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
              await  dto.ImageFile.CopyToAsync(stream);
            }
            return filePath;

        }


        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user=await _authRepository.GetByToken(token);
            if (user is null)
                return false;
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await _authRepository.SaveRefreshTokenAsync(user);
            return true;
        }
        
    }
}
