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
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .HasMaxLength(100);

            builder.Property(c=> c.Type)
                .HasConversion<int>();

            // Relationships
            builder.HasMany(c => c.Members)
                .WithOne(cm => cm.Conversation)
                .HasForeignKey(cm=> cm.ConversationId);

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId);

            //index
            builder.HasIndex(c => c.CreatedAt);
        }
    }
}
