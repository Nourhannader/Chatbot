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
    public class MessageAttachmentConfiguration
    : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(
            EntityTypeBuilder<MessageAttachment> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Caption)
                .HasMaxLength(2000);

            builder.HasOne(x => x.Message)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.StoredFile)
                .WithMany(x => x.MessageAttachments)
                .HasForeignKey(x => x.StoredFileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.MessageId,
                x.StoredFileId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.MessageId,
                x.Order
            });
        }
    }
}
