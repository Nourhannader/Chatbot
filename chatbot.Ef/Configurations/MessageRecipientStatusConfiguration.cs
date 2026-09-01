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
    public class MessageRecipientStatusConfiguration : IEntityTypeConfiguration<MessageRecipientStatus>
    {
        public void Configure(EntityTypeBuilder<MessageRecipientStatus> builder)
        {
            builder.Property(x => x.Status)
                .IsRequired();

            // Relationships

            builder.HasOne(x => x.Recipient)
                .WithMany(x => x.MessageRecipientStatuses)
                .HasForeignKey(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new
            {
                x.MessageId,
                x.RecipientId
            }).IsUnique();
        }
    }
}
