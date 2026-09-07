using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;

namespace chatbot.Ef.Services
{
    public class StickerService(IUnitOfWork unitOfWork) : IStickerService
    {
        public async Task<StickerDto> CreateAsync(CreateStickerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException(
                    "Sticker name is required.");

            if (string.IsNullOrWhiteSpace(dto.Url))
                throw new ArgumentException(
                    "Sticker URL is required.");

            var packExists =
                await unitOfWork.Stickers.ExistsAsync(dto.StickerPackId);
                    

            if (!packExists)
                throw new KeyNotFoundException(
                    "Sticker pack not found.");

            var sticker = new Sticker
            {
                Name = dto.Name.Trim(),

                Url = dto.Url.Trim(),

                Category = string.IsNullOrWhiteSpace(dto.Category)
                    ? null
                    : dto.Category.Trim(),

                StickerPackId = dto.StickerPackId
            };

            await unitOfWork.Stickers.AddAsync(sticker);

            await unitOfWork.SaveChangesAsync();

            return new StickerDto
            {
                Id = sticker.Id,
                Name = sticker.Name,
                Url = sticker.Url,
                Category = sticker.Category,
                StickerPackId = sticker.StickerPackId
            };
        }

        public async Task DeleteAsync(Guid stickerId)
        {
             var sticker = await unitOfWork.Stickers
            .GetByIdAsync(stickerId);

            if (sticker == null)
                throw new KeyNotFoundException(
                    "Sticker not found.");

            await unitOfWork.Stickers.DeleteAsync(sticker);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<StickerDto>> GetAllAsync()
        {
            var stickers = await unitOfWork.Stickers.GetAllAsync();
            return stickers.Select(sticker => new StickerDto
            {
                Id = sticker.Id,
                Name = sticker.Name,
                Url = sticker.Url,
                Category = sticker.Category,
                StickerPackId = sticker.StickerPackId

            }).ToList();

        }

        public async Task<StickerDto?> GetByIdAsync(Guid stickerId)
        {
            var sticker = await unitOfWork.Stickers
             .GetByIdAsync(stickerId);

            if (sticker == null)
                return null;

            return new StickerDto
            {
                Id = sticker.Id,
                Name = sticker.Name,
                Url = sticker.Url,
                Category = sticker.Category,
                StickerPackId = sticker.StickerPackId
            };
        }

        public async Task<List<StickerDto>> GetByPackIdAsync(Guid stickerPackId)
        {
            var stickers = await unitOfWork.Stickers
             .GetByPackIdAsync(stickerPackId);

            return stickers.Select(sticker => new StickerDto
            {
                Id = sticker.Id,
                Name = sticker.Name,
                Url = sticker.Url,
                Category = sticker.Category,
                StickerPackId = sticker.StickerPackId

            }).ToList();
        }
        

        public async Task<Message> SendStickerAsync(SendStickerDto dto, Guid userId)
        {
            // Check sticker
            var sticker = await unitOfWork.Stickers
                .GetByIdAsync(dto.StickerId);

            if (sticker == null)
                throw new KeyNotFoundException(
                    "Sticker not found.");

            // Check conversation
            var conversation = await unitOfWork.Conversations
                .GetByIdAsync(dto.ConversationId);

            if (conversation == null)
                throw new KeyNotFoundException(
                    "Conversation not found.");

            // Create message
            var message = new Message
            {
                SenderId = userId,

                ConversationId = dto.ConversationId,

                Content = string.Empty,

                MessageType = MessageType.Sticker,

                StickerId = dto.StickerId,

                SendAt = DateTime.UtcNow
            };

            await unitOfWork.Messages.AddAsync(message);

            await unitOfWork.SaveChangesAsync();

            return message;
        }
    }
}
