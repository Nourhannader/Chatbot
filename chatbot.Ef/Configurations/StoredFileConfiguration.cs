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
    public class StoredFileConfiguration
     : IEntityTypeConfiguration<StoredFile>
    {
        public void Configure(
            EntityTypeBuilder<StoredFile> builder)
        {

            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.Extension)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.StorageKey)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.FileUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.ProcessingError)
                .HasMaxLength(2000);


            // =================================
            // Uploaded By
            // =================================

            builder.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.SetNull);


            // =================================
            // Thumbnail self relationship
            // =================================

            builder.HasOne(x => x.ThumbnailFile)
                .WithOne(x => x.ThumbnailOf)
                .HasForeignKey<StoredFile>(
                    x => x.ThumbnailFileId)
                .OnDelete(DeleteBehavior.Restrict);


            // =================================
            // Indexes
            // =================================

            builder.HasIndex(x => x.StorageKey)
                .IsUnique();

            builder.HasIndex(x => x.UploadedById);

            builder.HasIndex(x => x.Category);

            builder.HasIndex(x => x.MediaType);

            builder.HasIndex(x => x.ProcessingStatus);
        }
    }
}
