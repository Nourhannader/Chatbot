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
            builder.Property(x => x.OriginalFileName)
             .HasMaxLength(255)
             .IsRequired();

            builder.Property(x => x.StoredFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Extension)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.StorageKey)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.FileUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.Provider)
                .HasConversion<int>();

            builder.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.StorageKey)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.IsDeleted,
                x.CreatedAt
            });
        }
    }
}
