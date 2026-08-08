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
    public class DeletedMessageForUserConfiguration : IEntityTypeConfiguration<DeletedMessageForUser>
    {
        public void Configure(EntityTypeBuilder<DeletedMessageForUser> builder)
        {
            builder.HasKey(x =>
            new
            {
                x.MessageId,
                x.UserId
            });

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
