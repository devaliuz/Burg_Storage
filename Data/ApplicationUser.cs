using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Burg_Storage.Models
{
    /// <summary>
    /// Application user entity for ASP.NET Core Identity using GUID keys.
    /// Extend this class with profile fields you want to store for each member.
    /// </summary>
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Human-friendly name shown in the UI (distinct from UserName).
        /// </summary>
        [MaxLength(128)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Flag to soft-disable accounts without deleting them.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// UTC timestamp when the account was created.
        /// </summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional note for admins (e.g., role rationale, onboarding info).
        /// </summary>
        [MaxLength(512)]
        public string? AdminNote { get; set; }

        // Example navigation for future file path logging (one-to-many).
        // Keep it nullable to avoid forcing a join if not used yet.
        public ICollection<UserFilePath>? FilePaths { get; set; }
    }
}
