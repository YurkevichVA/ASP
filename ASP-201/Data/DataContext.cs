﻿using Microsoft.EntityFrameworkCore;

namespace ASP_201.Data
{
    public class DataContext: DbContext
    {
        public DbSet<Entity.User>               Users               { get; set; }
        public DbSet<Entity.EmailConfirmToken>  EmailConfirmTokens  { get; set; }
        public DbSet<Entity.Section>            Sections            { get; set; }
        public DbSet<Entity.Theme>              Themes              { get; set; }
        public DbSet<Entity.Topic>              Topics              { get; set; }
        public DbSet<Entity.Rate>               Rates               { get; set; }
        public DbSet<Entity.Post>               Posts               { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.Rate>()
                .HasKey(
                    nameof(Entity.Rate.ItemId),         // Встановлення композитного ключа
                    nameof(Entity.Rate.UserID));

            // One-to-many Author(User)-Section
            modelBuilder.Entity<Entity.Section>()
                .HasOne(s => s.Author)  // Navy
                .WithMany()             // Empty
                .HasForeignKey(s => s.AuthorId);

            modelBuilder.Entity<Entity.Section>()
                .HasMany(s => s.RateList)
                .WithOne()
                .HasForeignKey(r => r.ItemId);

            modelBuilder.Entity<Entity.Theme>()
                .HasOne(s => s.Author)  // Navy
                .WithMany()             // Empty
                .HasForeignKey(s => s.AuthorId);

            modelBuilder.Entity<Entity.Theme>()
                .HasMany(t => t.RateList)
                .WithOne()
                .HasForeignKey(r => r.ItemId);

            modelBuilder.Entity<Entity.Topic>()
                .HasOne(s => s.Author)  // Navy
                .WithMany()             // Empty
                .HasForeignKey(s => s.AuthorId);

            modelBuilder.Entity<Entity.Post>()
                .HasOne(s => s.Author)  // Navy
                .WithMany()             // Empty
                .HasForeignKey(s => s.AuthorId);

            modelBuilder.Entity<Entity.Post>()
                .HasOne(s => s.Reply)  // Navy
                .WithMany()            // Empty
                .HasForeignKey(s => s.ReplyId);
        }
    }
}
