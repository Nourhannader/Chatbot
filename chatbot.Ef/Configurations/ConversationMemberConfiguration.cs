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

            builder.Property(x => x.IsBanned)
             .HasDefaultValue(false);

            builder.HasOne(x => x.Conversation)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.ConversationMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BannedBy)
                .WithMany(x => x.ConversationMembers)
                .HasForeignKey(x => x.BannedById)
                .OnDelete(DeleteBehavior.Restrict);



            // Prevent duplicate membership
            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.UserId
            })
            .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.IsBanned
            });
        }
    }
}
