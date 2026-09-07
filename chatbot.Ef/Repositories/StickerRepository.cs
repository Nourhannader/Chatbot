using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;


namespace chatbot.Ef.Repositories
{
    public class StickerRepository(ApplicationDbContext context) : IStickerRepository
    {
        public async Task AddAsync(Sticker entity)
        {
            await context.Stickers.AddAsync(entity);
        }

        public  async Task DeleteAsync(Sticker sticker)
        {
            context.Stickers.Remove(sticker);
           await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid stickerId)
        {
            return await context.Stickers.AnyAsync(s => s.Id == stickerId);
        }

        public async Task<List<Sticker>> GetAllAsync()
        {
            return await context.Stickers.Include(s => s.StickerPack).ToListAsync();
        }

        public async Task<Sticker?> GetByIdAsync(Guid id)
        {
            return await context.Stickers.Include(s => s.StickerPack).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Sticker>> GetByPackIdAsync(Guid stickerPackId)
        {
            return await context.Stickers.Where(s => s.StickerPackId == stickerPackId)
                .Include(s => s.StickerPack).ToListAsync();
        }

        public void Update(Sticker entity)
        {
            context.Stickers.Update(entity);
        }
    }
}
