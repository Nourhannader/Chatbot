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
    public class RefreshTokenConfiguration :IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(x => x.Token)
                .IsRequired();
            
            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //properties
            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsActive);
            // Indexes
            builder.HasIndex(x => x.Token)
                   .IsUnique();

            builder.HasIndex(x => new
            {
                x.Token,
                x.UserId
            }).IsUnique();
        }
    }
}
