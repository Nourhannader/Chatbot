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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(100);

            builder.Property(x => x.Body)
                .HasMaxLength(500);

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            // Indexes
            builder.HasIndex(x => new
            {
                x.UserId,
                x.IsRead,
                x.CreatedAt
            });
        }
    }
}
