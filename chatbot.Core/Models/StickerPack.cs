using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class StickerPack:BaseEntity
    {
        public string Title { get; set; }
            = string.Empty;
        public ICollection<Sticker> Stickers { get; set; }
            = new List<Sticker>();
    }
}
