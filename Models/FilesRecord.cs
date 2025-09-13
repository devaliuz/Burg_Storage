using System.ComponentModel.DataAnnotations;

namespace Burg_Storage.Models
{
    /// <summary>
    /// Represents metadata for a file uploaded to the system.
    /// </summary>
    public class FileRecord
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Original file name as uploaded by the user.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Relative or absolute path to the stored file on the server.
        /// </summary>
        [Required]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// File size in kilobytes.
        /// </summary>
        public long SizeKb { get; set; }

        /// <summary>
        /// ID of the user who uploaded the file.
        /// </summary>
        [Required]
        public string UploadedByUserId { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the file was uploaded.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
