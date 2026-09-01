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
    public class BlockListConfiguration : IEntityTypeConfiguration<BlockList>
    {
        public void Configure(EntityTypeBuilder<BlockList> builder)
        {
            

            // Relationships

            builder.HasOne(x => x.Blocker)
                .WithMany(x=> x.BlockedUsers)
                .HasForeignKey(x => x.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Blocked)
                .WithMany(x => x.BlockedByUsers)
                .HasForeignKey(x => x.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x =>
                new
                {
                    x.BlockerId,
                    x.BlockedId
                })
                .IsUnique();
        }
    }
}
