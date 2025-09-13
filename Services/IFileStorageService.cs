using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Burg_Storage.Models;

namespace Burg_Storage.Services
{
    /// <summary>
    /// Abstraction for storing and retrieving files along with their metadata.
    /// Keeps controllers decoupled from the concrete storage implementation.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves an uploaded file and persists its metadata.
        /// </summary>
        /// <param name="file">Incoming file from a multipart form.</param>
        /// <param name="uploadedByUserId">Identity user id of the uploader.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created <see cref="FileRecord"/>.</returns>
        Task<FileRecord> SaveAsync(IFormFile file, string uploadedByUserId, CancellationToken ct = default);

        /// <summary>
        /// Opens a stored file by its database id for downloading.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Tuple (stream, contentType, downloadFileName) or null if not found.</returns>
        Task<(Stream Stream, string ContentType, string DownloadFileName)?> GetAsync(int fileId, CancellationToken ct = default);

        /// <summary>
        /// Deletes a stored file and its metadata.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the file existed and was deleted; otherwise false.</returns>
        Task<bool> DeleteAsync(int fileId, CancellationToken ct = default);

        /// <summary>
        /// Lists files uploaded by the specified user (newest first).
        /// </summary>
        /// <param name="uploadedByUserId">Identity user id.</param>
        /// <param name="ct">Cancellation token.</param>
        Task<List<FileRecord>> ListByUserAsync(string uploadedByUserId, CancellationToken ct = default);

        /// <summary>
        /// Returns a public web-relative URL for the given record (e.g. /uploads/...).
        /// </summary>
        /// <param name="record">File metadata record.</param>
        string GetPublicUrl(FileRecord record);
    }
}
