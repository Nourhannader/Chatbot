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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
           

            builder.Property(x => x.Content)
                .HasMaxLength(4000);

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsDeletedForEveryone)
            .HasDefaultValue(false);

            builder.HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            // Relationships
            builder.HasMany(x => x.Reactions)
                .WithOne(x => x.Message)
                .HasForeignKey(x => x.MessageId);

            builder.HasMany(x => x.RecipientStatuses)
                .WithOne(x => x.Message)
                .HasForeignKey(x => x.MessageId);

            builder.HasMany(x => x.DeletedForUsers)
                .WithOne(x => x.Message)
                .HasForeignKey(x => x.MessageId);

            builder.HasOne(x => x.OriginalMessage)
              .WithMany(x => x.ForwardMessages)
              .HasForeignKey(x => x.OriginalMessageId)
              .OnDelete(DeleteBehavior.Restrict);

            // Indexes

            builder.HasIndex(x => x.Content);


            builder.HasIndex(x =>
                new
                {
                    x.ConversationId,
                    x.SentAt
                });
        }
    }
}
