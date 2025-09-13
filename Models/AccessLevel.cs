namespace Burg_Storage.Models
{
    /// <summary>
    /// Access rights for a stored document.
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>Only the owner can access the document.</summary>
        Private = 0,
        /// <summary>Shared with specific users.</summary>
        Shared = 1,
        /// <summary>Available to all authenticated users.</summary>
        Public = 2
    }
}
