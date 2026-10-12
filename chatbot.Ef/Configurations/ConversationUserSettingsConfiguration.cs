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
    public class ConversationUserSettingsConfiguration : IEntityTypeConfiguration<ConversationUserSettings>
    {
        public void Configure(EntityTypeBuilder<ConversationUserSettings> builder)
        {

            builder.Property(x => x.IsMuted)
                .HasDefaultValue(false);

            builder.Property(x => x.IsArchived)
                .HasDefaultValue(false);

            builder.Property(x => x.IsPinned)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Conversation)
                .WithMany(x => x.UserSettings)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One settings row per user/conversation
            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.UserId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.UserId,
                x.IsArchived
            });

            builder.HasIndex(x => new
            {
                x.UserId,
                x.IsPinned
            });
        }
    }
}
