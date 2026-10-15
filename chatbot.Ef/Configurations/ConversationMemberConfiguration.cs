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
    public sealed class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
    {
        public void Configure(EntityTypeBuilder<ConversationMember> builder)
        {

            // A user has only one membership row
            // per conversation.

            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.UserId
            })
            .IsUnique();

            builder.HasIndex(x => new
            {
                x.ConversationId,
                x.IsBanned
            });


            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.MutedBy)
                .WithMany()
                .HasForeignKey(x => x.MutedById)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.BannedBy)
                .WithMany()
                .HasForeignKey(x => x.BannedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.BanReason)
                .HasMaxLength(500);


        }
    }
}
