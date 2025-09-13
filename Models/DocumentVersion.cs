using System;
using System.ComponentModel.DataAnnotations;

namespace Burg_Storage.Models
{
    /// <summary>
    /// Stores a single version of a document referencing a file record.
    /// </summary>
    public class DocumentVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }
        public Document? Document { get; set; }

        [Required]
        public int FileRecordId { get; set; }
        public FileRecord? FileRecord { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
