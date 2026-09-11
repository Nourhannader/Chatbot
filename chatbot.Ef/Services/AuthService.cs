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
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace chatbot.Ef.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly JwtSettings jwt;
        private readonly IJwtService jwtService;
        private readonly ITokenHashService hashService;
        public AuthService(IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwt,IJwtService jwtService, ITokenHashService hashService)
        {
            this.unitOfWork = unitOfWork;
            this.jwt = jwt.Value;
            this.jwtService = jwtService;
            this.hashService = hashService;
        }
        //login
        public async Task<AuthResponseDto> GetTokenAsync(LoginDto model)
        {
           
            var user = await unitOfWork.Auth.GetByEmailAsync(model.Email);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            //check Password
            var validPassword = await unitOfWork.Auth.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
                throw new UnauthorizedAccessException("Invalid email or password");
            // Create Device Session
            var session = new DeviceSession
            {
                UserId = user.Id,

                DeviceId = model.DeviceId,

                DeviceType = model.DeviceType,

                DeviceName = model.DeviceName,

                CreatedAt = DateTime.UtcNow,

                LastActivityAt = DateTime.UtcNow,

                IsActive = true
            };
            await unitOfWork.Auth.CreateDeviceSessionAsync(session);
            var accessToken = await jwtService.GenerateAccessTokenAsync(user);

            var refreshToken =  jwtService.GenerateRefreshToken();

            // Save Refresh Token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,

                DeviceSessionId = session.Id,

                TokenHash =hashService.Hash(refreshToken),

                CreatedAt = DateTime.UtcNow,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(7)
            };

            await unitOfWork.Auth.SaveRefreshTokenAsync(refreshTokenEntity);

            await unitOfWork.SaveChangesAsync();
            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Refresh token is required");

            var tokenHash = hashService.Hash(token);
            //find token
            var refreshToken = await unitOfWork.Auth.GetRefreshTokenAsync(tokenHash);



            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");


            // Check token status
            if (!refreshToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token is expired or revoked");
            //check device session
            var session = refreshToken.DeviceSession;
            if (session == null)
                throw new UnauthorizedAccessException("Device session not found");

            if (!session.IsActive)
                throw new UnauthorizedAccessException("Device session is revoked");

            if (session.ExpiresAt.HasValue &&
                session.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Device session has expired");
            }
            var newAccessToken = await jwtService.GenerateAccessTokenAsync(refreshToken.User);
            var newRefreshToken =  jwtService.GenerateRefreshToken();
            var newRefreshTokenHash=hashService.Hash(newRefreshToken);
            var newRefreshTokenEntity =new RefreshToken
            {
                UserId = refreshToken.UserId,

                DeviceSessionId =
                    refreshToken.DeviceSessionId,

                TokenHash =
                    newRefreshTokenHash,

                CreatedAt =
                    DateTime.UtcNow,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(7)
            };
            //revoke old token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByTokenHash = newRefreshTokenHash;
            // Update session activity
            session.LastActivityAt =
                DateTime.UtcNow;


            // Save new token
            await unitOfWork.Auth
                .SaveRefreshTokenAsync(newRefreshTokenEntity);


            await unitOfWork.SaveChangesAsync();


            return new AuthResponseDto
            {
                AccessToken = newAccessToken,

                RefreshToken = newRefreshToken
            };

        }

        //register
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            if(await unitOfWork.Auth.GetByEmailAsync(model.Email) is not null)
                throw new Exception ("Email is already registered!" );
            if(await unitOfWork.Auth.GetByNameAsync(model.UserName) is not null)
                throw new Exception ("Username is already registered!" );
            var imageUrl = string.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0) {
                imageUrl = await GetImageUrl(model);
            }

            //create user
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone,
                
                ProfileImageUrl = imageUrl

            };
            var result = await unitOfWork.Auth.CreateUserAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)
                    );
                throw new Exception(errors);
            }
            //create session
            var session = new DeviceSession
            {
                UserId = user.Id,
                DeviceId=model.DeviceId,
                DeviceType=model.DeviceType,
                DeviceName=model.DeviceName,
                CreatedAt=DateTime.UtcNow,
                LastActivityAt=DateTime.UtcNow,
                IsActive=true
            };
            await unitOfWork.Auth.CreateDeviceSessionAsync(session);
            var accessToken = await jwtService.GenerateAccessTokenAsync(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            //create refreshtoken entity
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,

                DeviceSessionId = session.Id,

                TokenHash =hashService.Hash(refreshToken),

                CreatedAt = DateTime.UtcNow,

                ExpiresAt =
                DateTime.UtcNow.AddDays(7)
            };
            await unitOfWork.Auth.SaveRefreshTokenAsync(refreshTokenEntity);
            await unitOfWork.SaveChangesAsync();


            return new AuthResponseDto
            {
                AccessToken=accessToken,
                RefreshToken=refreshToken
            };

        }

        private async  Task<string> GetImageUrl(RegisterDto dto)
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

        //Revoke token , logout
        public async Task<bool> RevokeTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;


            var tokenHash =hashService.Hash(token);


            var refreshToken =await unitOfWork.Auth
                    .GetRefreshTokenAsync(tokenHash);


            if (refreshToken == null)
                return false;


            if (!refreshToken.IsActive)
                return false;


            // Revoke Refresh Token
            await unitOfWork.Auth
                .RevokeRefreshTokenAsync(refreshToken);


            // Optionally revoke the whole device session
            await unitOfWork.Auth
                .RevokeDeviceSessionAsync(refreshToken.DeviceSession);


            await unitOfWork.SaveChangesAsync();


            return true;
        }
        
    }
}
