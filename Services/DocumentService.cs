using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Burg_Storage.Data;
using Burg_Storage.Models;

namespace Burg_Storage.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IDocumentService"/> using EF Core and <see cref="IFileStorageService"/>.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileStorageService _fileStorage;

        public DocumentService(ApplicationDbContext db, IFileStorageService fileStorage)
        {
            _db = db;
            _fileStorage = fileStorage;
        }

        /// <inheritdoc />
        public async Task<Document> CreateAsync(string name, Guid ownerId, AccessLevel accessLevel, IFormFile file, CancellationToken ct = default)
        {
            var fileRecord = await _fileStorage.SaveAsync(file, ownerId.ToString(), ct);

            var document = new Document
            {
                Name = name,
                OwnerId = ownerId,
                AccessLevel = accessLevel
            };

            var version = new DocumentVersion
            {
                Document = document,
                FileRecordId = fileRecord.Id,
                VersionNumber = 1
            };

            document.Versions.Add(version);

            _db.Documents.Add(document);
            await _db.SaveChangesAsync(ct);
            return document;
        }

        /// <inheritdoc />
        public async Task<DocumentVersion> AddVersionAsync(int documentId, IFormFile file, CancellationToken ct = default)
        {
            var document = await _db.Documents.Include(d => d.Versions).FirstOrDefaultAsync(d => d.Id == documentId, ct);
            if (document == null)
                throw new InvalidOperationException($"Document {documentId} not found.");

            var fileRecord = await _fileStorage.SaveAsync(file, document.OwnerId.ToString(), ct);
            var nextVersion = document.Versions.Count == 0 ? 1 : document.Versions.Max(v => v.VersionNumber) + 1;

            var version = new DocumentVersion
            {
                DocumentId = documentId,
                FileRecordId = fileRecord.Id,
                VersionNumber = nextVersion
            };

            _db.DocumentVersions.Add(version);
            await _db.SaveChangesAsync(ct);
            return version;
        }

        /// <inheritdoc />
        public Task<List<Document>> ListByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        {
            return _db.Documents
                .Include(d => d.Versions)
                    .ThenInclude(v => v.FileRecord)
                .Where(d => d.OwnerId == ownerId)
                .ToListAsync(ct);
        }
    }
}
