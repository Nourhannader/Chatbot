using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IStickerService
    {
        Task<List<StickerDto>> GetAllAsync();

        Task<List<StickerDto>> GetByPackIdAsync(Guid stickerPackId);

        Task<StickerDto?> GetByIdAsync(Guid stickerId);

        Task<StickerDto> CreateAsync(CreateStickerDto dto);

        Task DeleteAsync(Guid stickerId);

        Task<Message> SendStickerAsync(SendStickerDto dto,Guid userId);
    }
}
