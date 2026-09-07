using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class Sticker:BaseEntity
    {
        public string Name { get; set; }
        = string.Empty;

        public string Url { get; set; }
            = string.Empty;

        public string Category { get; set; }
            = string.Empty;

        public Guid StickerPackId { get; set; }

        public StickerPack StickerPack { get; set; }
            = null!;
    }
}
