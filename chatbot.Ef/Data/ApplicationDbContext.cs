
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace chatbot.Ef.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        #region DbSets
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();

        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();
        public DbSet<MessageRecipientStatus> MessageRecipientStatuses => Set<MessageRecipientStatus>();
        public DbSet<DeletedMessageForUser> DeletedMessagesForUsers => Set<DeletedMessageForUser>();

        public DbSet<BlockList> BlockLists => Set<BlockList>();

        public DbSet<UserDevice> UserDevices => Set<UserDevice>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================
            // ConversationMember
            // ==========================
            builder.Entity<ConversationMember>()
                .HasKey(cm => new { cm.ConversationId, cm.UserId });

            builder.Entity<ConversationMember>()
                .HasOne(cm => cm.Conversation)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ConversationId);

            builder.Entity<ConversationMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ConversationMembers)
                .HasForeignKey(cm => cm.UserId);

            // ==========================
            // Message
            // ==========================
            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Message Recipient Status
            // ==========================
            builder.Entity<MessageRecipientStatus>()
                .HasKey(m => new { m.MessageId, m.RecipientId });

            builder.Entity<MessageRecipientStatus>()
                .HasOne(m => m.Message)
                .WithMany(m => m.RecipientStatuses)
                .HasForeignKey(m => m.MessageId);

            builder.Entity<MessageRecipientStatus>()
                .HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Message Reaction
            // ==========================
            builder.Entity<MessageReaction>()
                .HasOne(r => r.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MessageId);

            builder.Entity<MessageReaction>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageReaction>()
                .HasIndex(r => new { r.MessageId, r.UserId })
                .IsUnique();

            // ==========================
            // Delete For Me
            // ==========================
            builder.Entity<DeletedMessageForUser>()
                .HasKey(d => new { d.MessageId, d.UserId });

            builder.Entity<DeletedMessageForUser>()
                .HasOne(d => d.Message)
                .WithMany(m => m.DeletedForUsers)
                .HasForeignKey(d => d.MessageId);

            builder.Entity<DeletedMessageForUser>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // User Device
            // ==========================
            builder.Entity<UserDevice>()
                .HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId);

            // ==========================
            // Refresh Token
            // ==========================
            builder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId);

            // ==========================
            // Block List
            // ==========================
            builder.Entity<BlockList>()
                .HasOne(b => b.Blocker)
                .WithMany()
                .HasForeignKey(b => b.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BlockList>()
                .HasOne(b => b.Blocked)
                .WithMany()
                .HasForeignKey(b => b.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BlockList>()
                .HasIndex(b => new { b.BlockerId, b.BlockedId })
                .IsUnique();

            // ==========================
            // Indexes
            // ==========================
            builder.Entity<Message>()
                .HasIndex(m => new { m.ConversationId, m.SentAt });

            builder.Entity<Message>()
                .HasIndex(m => m.SentAt);

            builder.Entity<ConversationMember>()
                .HasIndex(cm => new { cm.UserId, cm.ConversationId });

            builder.Entity<ConversationMember>()
                .HasIndex(cm => new { cm.ConversationId, cm.UserId });

            builder.Entity<MessageRecipientStatus>()
                   .HasIndex(mrs => new { mrs.MessageId, mrs.RecipientId })
                   .IsUnique();
            builder.Entity<MessageRecipientStatus>()
                   .HasIndex(mrs => new { mrs.RecipientId, mrs.ReadAt });

            builder.Entity<Conversation>()
                   .HasIndex(c => new { c.Type, c.CreatedAt });

            builder.Entity<UserDevice>()
                .HasIndex(d => d.DeviceToken)
                .IsUnique();

        }

    }
}
