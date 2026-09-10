using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Services
{
    public class RefreshTokenService(IUnitOfWork unitOfWork,ITokenHashService tokenHash,IJwtService jwt) : IRefreshTokenService
    {
       
        public async Task<RefreshTokenResultDto> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is required.");
            var sessions = await unitOfWork.Sessions.GetAllActiveSession();

            DeviceSession? session = null;
            foreach(var item in sessions)
            {
                if (tokenHash.Verify(refreshToken, item.RefreshTokenHash))
                {
                    session = item;
                    break;
                }
            }
            if (session == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            //token rotation
            session.IsRevoked = true;
            session.RevokedAt = DateTime.UtcNow;
            //generate refreshtoken
            var newRefreshToken = jwt.GenerateRefreshToken();
            session.RefreshTokenHash = tokenHash.Hash(newRefreshToken);
            session.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            session.LastUsedAt = DateTime.UtcNow;
            var accessToken = await jwt.GenerateAccessTokenAsync(session.User);
            await unitOfWork.SaveChangesAsync();

            return new RefreshTokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

        }

        public async Task RevokeAllAsync(Guid userId)
        {
            if (userId == null)
                throw new ArgumentException("User ID is required.",nameof(userId));
            var sessions = await unitOfWork.Sessions.GetAllActiveSession(userId);
            var now=DateTime.UtcNow;
            foreach(var item in sessions)
            {
                item.IsRevoked = true;
                item.RevokedAt = now;
            }
            await unitOfWork.SaveChangesAsync();
        }

        public async Task RevokeAsync(Guid sessionId)
        {
            var session = await unitOfWork.Sessions.GetById(sessionId);
            if (session == null || session.IsRevoked)
                return;
            session.IsRevoked = true;
            session.RevokedAt= DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
