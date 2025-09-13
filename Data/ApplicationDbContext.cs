using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Burg_Storage.Models;

namespace Burg_Storage.Data;

/// <summary>
/// EF Core DbContext for the app, extending Identity with <see cref="ApplicationUser"/>.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance using the supplied options.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Table storing metadata for uploaded files.
    /// </summary>
    public DbSet<FileRecord> FileRecords => Set<FileRecord>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // FileRecord configuration (keep it minimal; expand later if needed)
        builder.Entity<FileRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            e.Property(x => x.FilePath).IsRequired();
        });
    }
}
