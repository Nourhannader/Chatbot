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
            builder.HasKey(x =>
            new
            {
                x.MessageId,
                x.RecipientId
            });

            // Relationships

            builder.HasOne(x => x.Recipient)
                .WithMany()
                .HasForeignKey(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
