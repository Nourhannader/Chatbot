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
    public class UserConnectionConfigration : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.Property(x => x.ConnectionId)
           .IsRequired()
           .HasMaxLength(200);

            builder.Property(x => x.DeviceType)
                .HasMaxLength(100);
            // Relationships
            builder.HasOne(x => x.User) 
                .WithMany() 
                .HasForeignKey(x => x.UserId) 
                .OnDelete(DeleteBehavior.NoAction);
            // Indexes
            builder.HasIndex(x => x.UserId);
            
            builder.HasIndex(x => x.ConnectionId) 
                .IsUnique();
        }
    }
}
