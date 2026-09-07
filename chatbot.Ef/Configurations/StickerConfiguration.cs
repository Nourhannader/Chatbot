using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace chatbot.Ef.Configurations
{
    public class StickerConfiguration :IEntityTypeConfiguration<Sticker>
    {
        public void Configure(EntityTypeBuilder<Sticker> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(50);
            // Relationships
            builder.HasOne(x => x.StickerPack)
                .WithMany(x => x.Stickers)
                .HasForeignKey(x => x.StickerPackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
