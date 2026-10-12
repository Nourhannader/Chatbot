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
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
           

            builder.Property(c => c.Title)
                .HasMaxLength(100);

            builder.Property(c=> c.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

            // Created By
            builder.HasOne(x => x.CreatedBy)
                .WithMany(u=> u.Conversations)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Deleted By
            builder.HasOne(x => x.DeletedBy)
                .WithMany(u=> u.Conversations)
                .HasForeignKey(x => x.DeletedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Group Image
            builder.HasOne(x => x.Image)
                .WithMany()
                .HasForeignKey(x => x.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Members
            builder.HasMany(x => x.Members)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Settings
            builder.HasMany(x => x.UserSettings)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Messages
            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            //index
            builder.HasIndex(x => x.CreatedById);

            builder.HasIndex(x => new
            {
                x.Type,
                x.IsDeleted
            });
        }
    }
}
