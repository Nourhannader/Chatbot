using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Reactions;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageReactionService
    {
        Task<ReactionResultDto> ToggleReactionAsync(Guid userId, AddReactionDto dto);

        Task<List<MessageReactionDto>> GetByMessageIdAsync(Guid userId,Guid messageId);

    }
}
