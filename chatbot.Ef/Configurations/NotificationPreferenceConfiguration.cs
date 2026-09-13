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
    public class NotificationPreferenceConfiguration:IEntityTypeConfiguration<NotificationPreferences>
    {
        public void Configure(EntityTypeBuilder<NotificationPreferences> builder)
        {

            builder.Property(x => x.NewMessages)
                .HasDefaultValue(true);

            builder.Property(x => x.MessageReactions)
                .HasDefaultValue(true);
            builder.Property(x => x.MessageReplies)
                .HasDefaultValue(true);

            builder.Property(x => x.Mentions)
                .HasDefaultValue(true);

            builder.Property(x => x.PushNotifications)
                .HasDefaultValue(true);

            builder.Property(x => x.SoundEnabled)
                .HasDefaultValue(true);

            builder.HasOne(x => x.User)
             .WithOne()
             .HasForeignKey<NotificationPreferences>(
                 x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

        }
    }
}
