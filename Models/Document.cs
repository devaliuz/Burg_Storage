using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Burg_Storage.Models
{
    /// <summary>
    /// Represents a logical document owned by a user with access control.
    /// </summary>
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        [Required]
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Private;

        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    }
}
