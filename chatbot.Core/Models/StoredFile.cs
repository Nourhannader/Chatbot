using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Core.Models
{
    public class StoredFile : BaseEntity
    {
        // ==============================
        // Original File Information
        // ==============================

        /// <summary>
        /// Original file name uploaded by user.
        /// Example: profile-photo.jpg
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; }
            = null!;


        /// <summary>
        /// Generated file name used internally.
        /// Example: 1f92a4d8....jpg
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; }
            = null!;


        /// <summary>
        /// Extension including dot.
        /// Example: .jpg
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Extension { get; set; }
            = null!;


        /// <summary>
        /// MIME type.
        /// Example: image/jpeg
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string ContentType { get; set; }
            = null!;


        /// <summary>
        /// File size in bytes.
        /// </summary>
        public long FileSize { get; set; }


        // ==============================
        // Storage
        // ==============================

        /// <summary>
        /// Storage provider.
        /// Local / Azure / S3 / MinIO
        /// </summary>
        public StorageProviderType Provider { get; set; }


        /// <summary>
        /// Stable storage identifier.
        ///
        /// Example:
        /// conversations/{conversationId}/abc.jpg
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string StorageKey { get; set; }
            = null!;


        /// <summary>
        /// Optional public/static URL.
        ///
        /// Do not rely on this for signed URLs.
        /// </summary>
        [MaxLength(2000)]
        public string? FileUrl { get; set; }


        // ==============================
        // Metadata
        // ==============================

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        /// <summary>
        /// User who uploaded the file.
        /// Null for system-generated files.
        /// </summary>
        public Guid? UploadedById { get; set; }

        public ApplicationUser? UploadedBy { get; set; }


        // ==============================
        // Image Metadata
        // ==============================

        public int? Width { get; set; }

        public int? Height { get; set; }


        // ==============================
        // Soft Delete
        // ==============================

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsPhysicallyDeleted { get; set; }

        //==============================
        //Message
        //=============================

        public Guid? MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
