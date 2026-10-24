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
        // =====================================
        // File Information
        // =====================================

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; }
            = null!;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; }
            = null!;

        [Required]
        [MaxLength(20)]
        public string Extension { get; set; }
            = null!;

        [Required]
        [MaxLength(150)]
        public string ContentType { get; set; }
            = null!;

        public long FileSize { get; set; }


        // =====================================
        // Classification
        // =====================================

        /// <summary>
        /// Business purpose:
        /// UserProfileImage,
        /// ConversationImage,
        /// MessageFile,
        /// Thumbnail
        /// </summary>
        public FileCategory Category { get; set; }

        /// <summary>
        /// Physical media type:
        /// Image,
        /// Video,
        /// Audio,
        /// Document,
        /// Other
        /// </summary>
        public MediaType MediaType { get; set; }


        // =====================================
        // Storage
        // =====================================

        public StorageProviderType Provider { get; set; }

        [Required]
        [MaxLength(1000)]
        public string StorageKey { get; set; } = null!;

        /// <summary>
        /// Optional cached/public URL.
        /// StorageKey remains the source of truth.
        /// </summary>
        [MaxLength(2000)]
        public string? FileUrl { get; set; }


        // =====================================
        // Upload
        // =====================================

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public Guid? UploadedById { get; set; }

        public ApplicationUser? UploadedBy { get; set; }


        // =====================================
        // Media Metadata
        // =====================================

        public int? Width { get; set; }

        public int? Height { get; set; }

        public double? DurationSeconds { get; set; }


        // =====================================
        // Thumbnail
        // =====================================

        public Guid? ThumbnailFileId { get; set; }

        public StoredFile? ThumbnailFile { get; set; }

        public StoredFile? ThumbnailOf { get; set; }


        // =====================================
        // Processing
        // =====================================

        public FileProcessingStatus ProcessingStatus { get; set; }
            = FileProcessingStatus.Pending;

        [MaxLength(2000)]
        public string? ProcessingError { get; set; }

        public DateTime? ProcessingStartedAt { get; set; }

        public DateTime? ProcessingCompletedAt { get; set; }


        // =====================================
        // Soft Delete
        // =====================================

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsPhysicallyDeleted { get; set; }


        // =====================================
        // Relationships
        // =====================================

        public ICollection<MessageAttachment> MessageAttachments { get; set; }
            = new List<MessageAttachment>();
    }
}
