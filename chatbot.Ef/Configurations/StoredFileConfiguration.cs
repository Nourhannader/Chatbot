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
    public class StoredFileConfiguration:IEntityTypeConfiguration<StoredFile>
    {
        public void Configure(EntityTypeBuilder<StoredFile> builder)
        {
            builder.Property(x => x.OriginalName)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(x => x.StoredName)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(x => x.Path)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.Size)
                .IsRequired();
            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            builder.HasOne(x => x.UploadedByUser)
                .WithMany(x => x.UploadedFiles)
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Message)
                .WithMany(x=> x.StoredFiles)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Restrict);

            //indexes
            builder.HasIndex(x => x.MessageId);

            builder.HasIndex(x => x.UploadedByUserId);
        }
    }
}
