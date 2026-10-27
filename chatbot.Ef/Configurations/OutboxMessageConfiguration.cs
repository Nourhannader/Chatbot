using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure( EntityTypeBuilder<OutboxMessage> builder)
        {
            

            builder.Property(x => x.EventType)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Payload)
                .IsRequired();

            builder.Property(x => x.LastError)
                .HasMaxLength(4000);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.EventId)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.Status,
                x.NextRetryAt,
                x.CreatedAt
            });
        }
    
}
}
