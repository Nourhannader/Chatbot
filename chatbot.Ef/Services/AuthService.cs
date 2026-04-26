using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;
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


            return authResponse;
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
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

        public Task<bool> RevokeTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
