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
            builder.HasOne(x=> x.User)
                .WithMany(c=> c.ConversationMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(x =>
                new
                {
                    x.UserId,
                    x.ConversationId
                }).IsUnique();
        }
    }
}
