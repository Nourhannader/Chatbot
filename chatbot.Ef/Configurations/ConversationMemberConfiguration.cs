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
    public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
    {
        public void Configure(EntityTypeBuilder<ConversationMember> builder)
        {
            builder.HasOne(x => x.Conversation)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x=> x.User)
                .WithMany(c=> c.ConversationMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Role
            builder.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();


            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.UserId
            })
         .IsUnique();

            // Useful indexes
            builder.HasIndex(x => x.ConversationId);

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.LeftAt
            });
        }
    }
}
