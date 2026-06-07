using LIN.Cloud.Identity.FileStorage.Models;

namespace LIN.Cloud.Identity.FileStorage.Abstractions;

public interface IFileStorageService
{
    Task<FileStorageResult> UploadFileAsync(string bucket, string objectName, Stream stream, long size, string contentType, CancellationToken cancellationToken = default);
    Task<FileStorageResult> DownloadFileAsync(string url, CancellationToken cancellationToken = default);
    Task<FileStorageResult> GetTemporaryUrlAsync(string url, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<FileStorageResult> DeleteFileAsync(string url, CancellationToken cancellationToken = default);
}