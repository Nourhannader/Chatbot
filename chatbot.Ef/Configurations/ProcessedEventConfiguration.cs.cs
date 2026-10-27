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
    public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
        {
            builder.HasKey(x => new
            {
                x.EventId,
                x.HandlerName
            });
            builder.Property(x => x.HandlerName)
            .HasMaxLength(500)
            .IsRequired();

            builder.HasIndex(x => x.ProcessedAt);

        }
    }
}
