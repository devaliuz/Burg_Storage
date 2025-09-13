using System.ComponentModel.DataAnnotations;

namespace Burg_Storage.Models
{
    /// <summary>
    /// Example entity to log storage path ownership per user.
    /// This will back the "who owns which storage path" requirement.
    /// </summary>
    public sealed class UserFilePath
    {
        /// <summary>
        /// Primary key (GUID).
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Owning user id (GUID).
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Owning user navigation.
        /// </summary>
        [Required]
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// Absolute or virtual path as stored in your storage layer.
        /// </summary>
        [Required, MaxLength(1024)]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// When the path was first registered.
        /// </summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional label/category (e.g., "inbox", "team-share", "archive").
        /// </summary>
        [MaxLength(64)]
        public string? Label { get; set; }
    }
}