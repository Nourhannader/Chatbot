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


            // Relationships
            builder.HasOne(x => x.User) 
                .WithMany(x => x.Connections) 
                .HasForeignKey(x => x.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UserDevice)
           .WithMany(x => x.Connections)
           .HasForeignKey(x => x.UserDeviceId)
           .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.UserId);
            
            builder.HasIndex(x => x.ConnectionId) 
                .IsUnique();
        }
    }
}
