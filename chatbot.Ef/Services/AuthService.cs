using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Enums;
using chatbot.Core.Exceptions;
using chatbot.Core.Helper;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.Repositories;
using FirebaseAdmin.Messaging;
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
        private readonly IStorageService storageService;
        public AuthService(IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwt,IJwtService jwtService, ITokenHashService hashService, IStorageService storageService)
        {
            this.unitOfWork = unitOfWork;
            this.jwt = jwt.Value;
            this.jwtService = jwtService;
            this.hashService = hashService;
            this.storageService = storageService;
        }
        
        private void RevokeDeviceSession(DeviceSession session, string? ipAddress)
        {
            var now = DateTime.Now;
            session.RevokedAt = now;
            foreach (var token in session.RefreshTokens)
            {
                token.RevokedByIp = ipAddress;
                token.RevokedAt = now;
            }

        }

        private async Task<AuthResponseDto>CreateAuthResponseAsync(ApplicationUser user,string deviceId,
            string? deviceName,DeviceType deviceType,string? ipAddress)
        {
            //generate accesstoken
            var accessToken = await jwtService.GenerateAccessTokenAsync(user);
            //generate refresh token
            var refreshToken = jwtService.GenerateRefreshToken();
            //hash refresh token
            var refreshTokenHash = hashService.Hash(refreshToken);
            //create device session
            var deviceSession = new DeviceSession
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                DeviceId = deviceId,

                DeviceName =deviceName ?? "Unknown Device",

                DeviceType = deviceType,

                IpAddress = ipAddress,

                CreatedAt =DateTime.UtcNow,

                LastActivityAt =DateTime.UtcNow,

                ExpiresAt =DateTime.UtcNow.AddDays(jwt.RefreshTokenExpirationDays)
            };
            //create refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                DeviceSessionId =deviceSession.Id,

                TokenHash =refreshTokenHash,

                CreatedAt =DateTime.UtcNow,

                ExpiresAt =DateTime.UtcNow.AddDays(jwt.RefreshTokenExpirationDays),

                CreatedByIp =ipAddress
            };
            //save
           await unitOfWork.Auth.AddDeviceSessionAsync(deviceSession);
           await unitOfWork.Auth.AddRefreshTokenAsync(refreshTokenEntity);
           await unitOfWork.SaveChangesAsync();
            //update user state
            user.IsOnline = true;
            user.LastSeenAt = DateTime.UtcNow;
            await unitOfWork.Auth.updateState(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken,

                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(jwt.AccessTokenExpirationMinutes),

                RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt,

                DeviceSessionId = deviceSession.Id
            };
        }
        //register
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress)
        {
            if (await unitOfWork.Auth.GetByEmailAsync(dto.Email) is not null)
                throw new ConflictException("Email is already registered!");
            if (await unitOfWork.Auth.GetByNameAsync(dto.UserName) is not null)
                throw new ConflictException("Username is already taken.");

            //create user
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                IsOnline = true,
                LastSeenAt = DateTime.UtcNow,
                ReadReceiptsEnabled = true,
                LastSeenVisible = true,
                IsTypingVisible = true
            };
            //identity create
            var result = await unitOfWork.Auth.CreateUserAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)
                    );
                throw new BadRequestException(errors);
            }

            //add image 
            Guid? uploadedImageId = null; 
            try
            { 
                if (dto.ImageFile is not null && dto.ImageFile.Length > 0) 
                {
                    var image = await storageService.UploadUserProfileImageAsync( dto.ImageFile, user.Id); 
                    
                    if (!image.Success) 
                    {
                        throw new BadRequestException( "Failed to upload profile image.");
                    }
                    uploadedImageId = image.FileId;
                    
                    user.ProfileImageId = image.FileId;
                    
                    await unitOfWork.Auth.updateState(user); 
                } 
                // 6. Add default role
                var roleResult = await unitOfWork.Auth.AddToRoleAsync( user, "User");
                await unitOfWork.Auth.AddToRoleAsync(user, "User");
                
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(x => x.Description));
                    throw new BadRequestException(errors);
                }
                return
                    await CreateAuthResponseAsync( user, dto.DeviceId, dto.DeviceName, dto.DeviceType, ipAddress);
            }
            catch { 
                
                // Cleanup uploaded image if registration fails
                    if (uploadedImageId.HasValue) {
                    try
                    {
                        await storageService.SoftDeleteAsync(uploadedImageId.Value);
                    }
                    catch
                    {
                        // Do not hide the original exception

                    } 
                } throw; 
            
            }
        }
            

        //login
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto,string? ipAddress)
        {
           
            var user = await unitOfWork.Auth.GetByEmailAsync(dto.Email);
            if (user is null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            //check Password
            var validPassword = await unitOfWork.Auth.CheckPasswordAsync(user, dto.Password);
            if (!validPassword)
                throw new UnauthorizedException("Invalid email or password");
            //create authentication response

            return await CreateAuthResponseAsync(user, dto.DeviceId, dto.DeviceName, dto.DeviceType, ipAddress);
           
        }

        //refresh token
        public async Task<AuthResponseDto> RefreshAsync(string token,string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException("Refresh token is required.");
            }
            //hash incoming token
            var tokenHash = hashService.Hash(token);

            //find token
            var oldToken = await unitOfWork.Auth.GetRefreshTokenAsync(tokenHash);


            if (oldToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            //get device session
            var session = await unitOfWork.Auth.GetDeviceSessionWithTokensAsync(oldToken.DeviceSessionId);
            if (session == null)
            {
                throw new UnauthorizedException("Device session not found.");
            }

            //detect token reuse
            if (oldToken.IsRevoked)
            {
                if (oldToken.IsRevoked)
                {
                    if (!string.IsNullOrWhiteSpace(oldToken.ReplacedByTokenHash))
                    {
                        RevokeDeviceSession(session, ipAddress);

                        await unitOfWork.SaveChangesAsync();

                        throw new RefreshTokenReuseException();
                    }

                    throw new UnauthorizedException("Refresh token has been revoked.");
                }
            }
            //check expiration
            if (oldToken.IsExpired)
            {
                throw new UnauthorizedException("Refresh token has expired.");
            }
            //check  session
            if (!session.IsActive)
                throw new UnauthorizedException("Device session is no longer active.");

           
            if (session.ExpiresAt.HasValue &&
                session.ExpiresAt <= DateTime.UtcNow)
            {

                session.RevokedAt = DateTime.UtcNow;

                await unitOfWork.SaveChangesAsync();

                throw new UnauthorizedException("Device session has expired.");
            }
            //get user
            var user = await unitOfWork.Auth.GetByIdAsync(oldToken.UserId);
            if (user == null)
            {
                throw new UnauthorizedException("User not found.");
            }
            //generate new tokens
            var newAccessToken = await jwtService.GenerateAccessTokenAsync(user);
            var newRefreshToken =  jwtService.GenerateRefreshToken();
            var newRefreshTokenHash=hashService.Hash(newRefreshToken);

            //revoke old token
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.RevokedByIp = ipAddress;
            oldToken.ReplacedByTokenHash = newRefreshTokenHash;
            //create new token
            var newTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                DeviceSessionId =session.Id,

                TokenHash =newRefreshTokenHash,

                CreatedAt =DateTime.UtcNow,

                ExpiresAt =DateTime.UtcNow.AddDays(jwt.RefreshTokenExpirationDays),

                CreatedByIp =ipAddress
            };

            
            // Update session activity
            session.LastActivityAt =DateTime.UtcNow;
            session.ExpiresAt = newTokenEntity.ExpiresAt;


            // Save new token
            await unitOfWork.Auth.AddRefreshTokenAsync(newTokenEntity);

            await unitOfWork.SaveChangesAsync();


            return new AuthResponseDto
            {
                AccessToken = newAccessToken,

                RefreshToken = newRefreshToken,

                AccessTokenExpiresAt =DateTime.UtcNow.AddMinutes(jwt.AccessTokenExpirationMinutes),

                RefreshTokenExpiresAt =newTokenEntity.ExpiresAt,

                DeviceSessionId =session.Id
            };

        }

        // logout current device
        public async Task LogoutAsync(string token, string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException(
                    "Refresh token is required.");
            }

            //hashToken
            var tokenHash = hashService.Hash(token);
            //findToken
            var refreshToken = await unitOfWork.Auth.GetRefreshTokenAsync(tokenHash);

            if (refreshToken == null)
            {
                return;
            }
            //get session
            var session
                = await unitOfWork.Auth.GetDeviceSessionWithTokensAsync(refreshToken.DeviceSessionId);
            if (session == null)
            {
                return;
            }
            //revoke session
            RevokeDeviceSession(session, ipAddress);
            //save
            await unitOfWork.SaveChangesAsync();
        }

        //logout all devices
        public async Task LogoutAllAsync(Guid userId, string? ipAddress)
        {
            var sessions = await unitOfWork.Auth.GetActiveDeviceSessionsAsync(userId);
            foreach(var session in sessions)
            {
                RevokeDeviceSession(session, ipAddress);
            }
            await unitOfWork.SaveChangesAsync();
        }

    }
}
