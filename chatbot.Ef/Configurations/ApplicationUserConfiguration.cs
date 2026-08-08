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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(50);
            builder.Property(u => u.LastName)
                .HasMaxLength(50);
            builder.Property(u => u.Bio)
                .HasMaxLength(500);

            // Relationships
            //conversation table
            builder.HasMany(u => u.ConversationMembers)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId);
            //message table
            builder.HasMany(u => u.MessagesSent)
                .WithOne(u => u.Sender)
                .HasForeignKey(u => u.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            //device table
            builder.HasMany(u => u.Devices)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId);

            //refresh token table
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId);

        }
    }
}
