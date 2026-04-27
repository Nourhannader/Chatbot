
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

namespace chatbot.Ef.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<chat> Chats { get; set; }
        public DbSet<ChatMembers> ChatMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<BlockList> BlockLists { get; set; }
        public DbSet<ChatbotResponse> ChatbotResponses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           

            builder.Entity<ChatMembers>()
           .HasOne(cm => cm.user)
           .WithMany(u => u.ChatMembers)
           .HasForeignKey(cm => cm.UserId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatMembers>()
                .HasOne(cm => cm.Chat)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatMembers>()
                .HasIndex(cm => new { cm.ChatId, cm.UserId })
                .IsUnique();

            builder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
           .HasOne(m => m.ReplyTo)
           .WithMany()
           .HasForeignKey(m => m.ReplyToMessageId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageReaction>()
           .HasOne(r => r.Message)
           .WithMany(m => m.Reactions)
           .HasForeignKey(r => r.MessageId);

            builder.Entity<MessageReaction>()
                .HasOne(r => r.user)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            builder.Entity<UserDevice>()
            .HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId);

            builder.Entity<BlockList>()
            .HasIndex(b => new { b.BlockerId, b.BlockedId })
            .IsUnique();

        }

    }
}
