using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Exceptions;
using chatbot.Core.Models;
using System.Management;
using System.Security.Claims;
using AutoMapper;
using System.Runtime.CompilerServices;
using chatbot.Core.Authorization.Conversation;
using chatbot.Core.DTOs;
using chatbot.Core.Common;
using chatbot.Core.Enums;

namespace chatbot.Ef.Services
{
    public class ConversationSettingsService(IUnitOfWork unitOfWork,IConversationAuthorizationService authorizationService
        ,IMapper mapper,TimeProvider timeProvider) : IConversationSettingsService
    {
        public async Task ArchiveAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var settings = await GetSettingsAsync(user, conversationId);
            if (settings.IsArchived)
                return;
            settings.IsArchived = true;
            settings.ArchivedAt=DateTime.UtcNow;
           await unitOfWork.SaveChangesAsync();
        }

        public async Task MuteAsync(ClaimsPrincipal user, Guid conversationId, MuteDuration? duration)
        {
            await AuthorizeAsync(user, conversationId);
            var now = timeProvider.GetUtcNow().UtcDateTime;

            // Removed invalid comparison between MuteDuration? and DateTime
            if (duration.HasValue && duration.Value == MuteDuration.Forever)
            {
                throw new BadRequestException("Mute expiration must be in the future.");
            }

            var settings = await GetSettingsAsync(user, conversationId);
            settings.IsMuted = false;
            settings.MuteUntil = null;

            switch (duration)
            {
                case MuteDuration.OneHour:
                    settings.MuteUntil = DateTime.UtcNow.AddHours(1);
                    break;

                case MuteDuration.EightHours:
                    settings.MuteUntil = DateTime.UtcNow.AddHours(8);
                    break;

                case MuteDuration.OneWeek:
                    settings.MuteUntil = DateTime.UtcNow.AddDays(7);
                    break;

                case MuteDuration.Forever:
                    settings.IsMuted = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(duration), duration, "Invalid mute duration.");
            }
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsMutedAsync(ClaimsPrincipal user, Guid conversationId)
        {
            var userId = user.GetUserId();
            var settings = await unitOfWork.Settings
                    .GetAsync(userId, conversationId);

            if (settings is null)
                return false;

            if (settings.IsMuted)
                return true;

            return settings.MuteUntil.HasValue &&
                   settings.MuteUntil.Value >
                   DateTime.UtcNow;
        }
       
        public async Task UnarchiveAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var settings = await GetSettingsAsync(user, conversationId);
            if (!settings.IsArchived)
                return;
            settings.IsArchived = false;
            settings.ArchivedAt = null;
           await unitOfWork.SaveChangesAsync();
        }

        public async Task UnmuteAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var settings = await GetSettingsAsync(user, conversationId);
            settings.IsMuted = false;
            settings.MuteUntil = null;
           await unitOfWork.SaveChangesAsync();
        }
       
        public async Task<ConversationSettingsDto> GetAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var userId = user.GetUserId();
            var settings = await unitOfWork.Settings.GetAsync(userId, conversationId);

            if (settings is null)
            {
                return new ConversationSettingsDto
                {
                    IsMuted = false,
                    MuteUntil = null,
                    IsArchived = false,
                    ArchivedAt = null,
                    IsPinned = false,
                    PinnedAt = null
                };
                
            }
            return mapper.Map<ConversationSettingsDto>(settings);
        }

        public async Task PinAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var settings = await GetSettingsAsync(user, conversationId);

            if (settings.IsPinned)
            {
                return;
            }
            settings.IsPinned = true;
            settings.PinnedAt = timeProvider.GetUtcNow().UtcDateTime;
            await unitOfWork.SaveChangesAsync();
            
        }

        public async Task UnPinAsync(ClaimsPrincipal user, Guid conversationId)
        {
            await AuthorizeAsync(user, conversationId);
            var settings = await GetSettingsAsync(user, conversationId);

            if (!settings.IsPinned)
            {
                return;
            }
            settings.IsPinned = false;
            settings.PinnedAt = null;
            await unitOfWork.SaveChangesAsync(); 
        }
        private async Task<ConversationUserSettings> GetSettingsAsync(ClaimsPrincipal user,Guid conversationId) 
        {
            var userId = user.GetUserId();
            return await unitOfWork.Settings.GetOrCreateSettingsAsync(userId, conversationId); 
        
        }
        private Task AuthorizeAsync(ClaimsPrincipal user,Guid conversationId)
        {
            return authorizationService.AuthorizeAsync(user, conversationId, ConversationPermissions.View);
        }
    }
}
