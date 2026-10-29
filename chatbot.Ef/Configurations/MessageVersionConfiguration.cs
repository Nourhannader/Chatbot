using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Configurations
{
    public class MessageVersionConfiguration : IEntityTypeConfiguration<MessageVersion>
    {
        public void Configure( EntityTypeBuilder<MessageVersion> builder)
        {

            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.Version)
                .IsRequired();

            builder.HasOne(x => x.Message)
                .WithMany(x => x.Version)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.MessageId,
                x.Version
            })
            .IsUnique();
        }
    }
}
