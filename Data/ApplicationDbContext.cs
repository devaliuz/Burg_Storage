using System;
using Burg_Storage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Burg_Storage.Data
{
    /// <summary>
    /// EF Core DbContext integrating ASP.NET Core Identity (GUID keys) and app entities.
    /// Target provider: SQLite. Configure in Program.cs via AddDbContext with UseSqlite.
    /// </summary>
    public sealed class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// EF-required constructor receiving configured options.
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Logs storage path ownership per user.
        /// </summary>
        public DbSet<UserFilePath> UserFilePaths => Set<UserFilePath>();

        /// <summary>
        /// Logical documents uploaded by users.
        /// </summary>
        public DbSet<Document> Documents => Set<Document>();

        /// <summary>
        /// Individual versions belonging to documents.
        /// </summary>
        public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();

        /// <summary>
        /// Model configuration for Identity and application entities.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity: keep default table names or set custom names if you prefer.
            // Example: builder.Entity<ApplicationUser>().ToTable("Users");
            // For now we keep defaults to reduce friction.

            // ApplicationUser: add indices that help lookups.
            builder.Entity<ApplicationUser>(e =>
            {
                e.HasIndex(u => u.DisplayName);
                e.HasIndex(u => u.IsActive);
                e.Property(u => u.DisplayName).HasMaxLength(128);
                e.Property(u => u.AdminNote).HasMaxLength(512);
            });

            // UserFilePath: enforce uniqueness per (UserId, Path) and configure FK.
            builder.Entity<UserFilePath>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Path)
                    .IsRequired()
                    .HasMaxLength(1024);

                e.Property(x => x.Label)
                    .HasMaxLength(64);

                e.HasIndex(x => new { x.UserId, x.Path })
                    .IsUnique();

                e.HasOne(x => x.User)
                    .WithMany(u => u.FilePaths)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Document: basic metadata and access control.
            builder.Entity<Document>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Name).IsRequired().HasMaxLength(255);
                e.HasMany(d => d.Versions)
                    .WithOne(v => v.Document!)
                    .HasForeignKey(v => v.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DocumentVersion: ensure sequential versions per document.
            builder.Entity<DocumentVersion>(e =>
            {
                e.HasKey(v => v.Id);
                e.HasIndex(v => new { v.DocumentId, v.VersionNumber }).IsUnique();
                e.HasOne(v => v.FileRecord)
                    .WithMany()
                    .HasForeignKey(v => v.FileRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SQLite-specific fine-tuning (optional but helpful):
            // - Ensure DateTime stored as TEXT in UTC if needed; EF defaults are fine.
            // - Keep string lengths constrained to avoid unnecessary storage.
        }

        /// <summary>
        /// Optional provider-specific tuning for SQLite. Call base if you override externally.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only apply defaults if not already configured by DI.
            if (!optionsBuilder.IsConfigured)
            {
                // Example fallback (replace with your connection string as needed):
                // optionsBuilder.UseSqlite("Data Source=./App_Data/burg_storage.db");
            }
        }
    }
}
