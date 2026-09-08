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
    public class MessageDeletionConfiguration
    : IEntityTypeConfiguration<MessageDeletion>
    {
        public void Configure(
            EntityTypeBuilder<MessageDeletion> builder)
        {
            builder.HasKey(x =>
                new
                {
                    x.MessageId,
                    x.UserId
                });

            builder.HasOne(x => x.Message)
                .WithMany(x => x.Deletions)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}
