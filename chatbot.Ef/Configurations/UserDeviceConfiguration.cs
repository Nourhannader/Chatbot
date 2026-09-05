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
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceType)
                .HasMaxLength(100);

            builder.Property(x => x.DeviceToken)
            .IsRequired()
            .HasMaxLength(500);


            // Device Type
            builder.Property(x => x.DeviceType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);


            // Device Name
            builder.Property(x => x.DeviceName)
                .HasMaxLength(100);

            //relationships
            builder.HasOne(x => x.User)
            .WithMany(x => x.Devices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Connections)
            .WithOne(x => x.UserDevice)
            .HasForeignKey(x => x.UserDeviceId)
            .OnDelete(DeleteBehavior.Cascade);

            //indexes
            builder.HasIndex(x => new
            {
                x.UserId,
                x.DeviceToken
            }).IsUnique();
        }
    }
}
