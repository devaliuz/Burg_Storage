using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Burg_Storage.Models;

namespace Burg_Storage.Services
{
    /// <summary>
    /// Service abstraction for managing documents and their versions.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Creates a new document with an initial file version.
        /// </summary>
        Task<Document> CreateAsync(string name, Guid ownerId, AccessLevel accessLevel, IFormFile file, CancellationToken ct = default);

        /// <summary>
        /// Adds a new version to an existing document.
        /// </summary>
        Task<DocumentVersion> AddVersionAsync(int documentId, IFormFile file, CancellationToken ct = default);

        /// <summary>
        /// Lists all documents owned by the specified user.
        /// </summary>
        Task<List<Document>> ListByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    }
}
