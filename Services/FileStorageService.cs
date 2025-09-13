using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Burg_Storage.Data;
using Burg_Storage.Models;

namespace Burg_Storage.Services
{
    /// <summary>
    /// Stores files under wwwroot/uploads and persists metadata to the database.
    /// Implements IFileStorageService so controllers can depend on the abstraction.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<FileStorageService> _logger;

        // Basic allowlist; extend as needed.
        private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".xlsx", ".png", ".jpg", ".jpeg", ".txt"
        };

        private const string UploadRootFolder = "uploads";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageService"/> class.
        /// </summary>
        public FileStorageService(IWebHostEnvironment env, ApplicationDbContext db, ILogger<FileStorageService> logger)
        {
            _env = env;
            _db = db;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<FileRecord> SaveAsync(IFormFile file, string uploadedByUserId, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            var safeFileName = MakeSafeFileName(file.FileName);
            var ext = Path.GetExtension(safeFileName);
            if (!_allowedExtensions.Contains(ext))
                throw new ArgumentException($"File type '{ext}' is not allowed.", nameof(file));

            // Directory: uploads/{userId}/{yyyy}/{MM}
            var relDirectory = Path.Combine(UploadRootFolder, uploadedByUserId, DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
            var absDirectory = Path.Combine(_env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), relDirectory);
            Directory.CreateDirectory(absDirectory); // ensure path exists

            // Unique file name
            var uniqueName = $"{Guid.NewGuid():N}-{safeFileName}";
            var absPath = Path.Combine(absDirectory, uniqueName);
            var relPath = "/" + Path.Combine(relDirectory, uniqueName).Replace('\\', '/');

            await using (var stream = new FileStream(absPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            {
                await file.CopyToAsync(stream, ct);
            }

            var record = new FileRecord
            {
                FileName = safeFileName,
                FilePath = relPath,
                SizeKb = Math.Max(1, file.Length / 1024),
                UploadedByUserId = uploadedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Set<FileRecord>().Add(record);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Saved file {File} for user {UserId} at {Path}", safeFileName, uploadedByUserId, relPath);
            return record;
        }

        /// <inheritdoc />
        public async Task<(Stream Stream, string ContentType, string DownloadFileName)?> GetAsync(int fileId, CancellationToken ct = default)
        {
            var record = await _db.Set<FileRecord>().AsNoTracking().FirstOrDefaultAsync(f => f.Id == fileId, ct);
            if (record == null) return null;

            var absPath = ToAbsolutePath(record.FilePath);
            if (!File.Exists(absPath))
            {
                _logger.LogWarning("File record {Id} exists but physical file is missing at {Path}", fileId, absPath);
                return null;
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(record.FileName, out var contentType))
                contentType = "application/octet-stream";

            var stream = new FileStream(absPath, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, useAsync: true);
            return (stream, contentType, record.FileName);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int fileId, CancellationToken ct = default)
        {
            var record = await _db.Set<FileRecord>().FirstOrDefaultAsync(f => f.Id == fileId, ct);
            if (record == null) return false;

            var absPath = ToAbsolutePath(record.FilePath);

            _db.Remove(record);
            await _db.SaveChangesAsync(ct);

            try
            {
                if (File.Exists(absPath))
                    File.Delete(absPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete physical file for record {Id} at {Path}", fileId, absPath);
            }

            _logger.LogInformation("Deleted file record {Id}", fileId);
            return true;
        }

        /// <inheritdoc />
        public Task<List<FileRecord>> ListByUserAsync(string uploadedByUserId, CancellationToken ct = default)
        {
            return _db.Set<FileRecord>()
                      .AsNoTracking()
                      .Where(f => f.UploadedByUserId == uploadedByUserId)
                      .OrderByDescending(f => f.CreatedAt)
                      .ToListAsync(ct);
        }

        /// <inheritdoc />
        public string GetPublicUrl(FileRecord record) => record.FilePath;

        // ---- helpers -------------------------------------------------------

        private string ToAbsolutePath(string webRelativePath)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var trimmed = webRelativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(webRoot, trimmed);
        }

        private static string MakeSafeFileName(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);

            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            name = string.Join("_", name.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            return $"{name}{ext}".Trim();
        }
    }
}
