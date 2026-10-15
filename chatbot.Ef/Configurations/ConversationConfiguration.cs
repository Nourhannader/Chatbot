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
    public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {

            builder.Property(x => x.Title)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);


            // Private conversation uniqueness

            builder.HasIndex(x =>
                    x.PrivateConversationKey)
                .IsUnique()
                .HasFilter(
                    "[PrivateConversationKey] IS NOT NULL");


            // Creator

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);


            // Deleted By

            builder.HasOne(x => x.DeletedBy)
                .WithMany()
                .HasForeignKey(x => x.DeletedById)
                .OnDelete(DeleteBehavior.Restrict);


            // Image

            builder.HasOne(x => x.Image)
                .WithMany()
                .HasForeignKey(x => x.ImageId)
                .OnDelete(DeleteBehavior.SetNull);


            // Members

            builder.HasMany(x => x.Members)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);


            // Messages

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);


            // Settings

            builder.HasMany(x => x.UserSettings)
                .WithOne(x => x.Conversation)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);


            // Soft delete

            builder.HasQueryFilter(x =>
                !x.IsDeleted);

            builder.HasIndex(x => new
            {
                x.Type,
                x.IsDeleted
            });

        }
    }
}
