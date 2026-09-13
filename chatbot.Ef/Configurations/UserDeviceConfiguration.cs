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

            builder.Property(x => x.PushToken)
            .IsRequired()
            .HasMaxLength(1000);

            builder.Property(x => x.Provider)
                .HasConversion<int>();

            builder.Property(x => x.DeviceType)
                .HasConversion<int>();

            builder.Property(x => x.DeviceName)
                .HasMaxLength(200);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.LastUsedAt)
                .IsRequired();

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
                x.PushToken,
                x.Provider
            }).IsUnique();

            builder.HasIndex(x => x.UserId);
        }
    }
}
