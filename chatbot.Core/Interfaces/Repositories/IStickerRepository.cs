using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IStickerRepository:IBaseRepository<Sticker, Guid>
    {
        Task<List<Sticker>> GetByPackIdAsync(Guid stickerPackId);
        Task<List<Sticker>> GetAllAsync();
        Task DeleteAsync(Sticker sticker);

        Task<bool> ExistsAsync(Guid stickerId);
    }
}
