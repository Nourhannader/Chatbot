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
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                .HasMaxLength(4000);

            builder.Property(x => x.Type)
                .HasConversion<int>();

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

            // Indexes

            builder.HasIndex(x =>
                new
                {
                    x.ConversationId,
                    x.SentAt
                });
        }
    }
}
