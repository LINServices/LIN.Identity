using LIN.Cloud.Identity.FileStorage.Models;

namespace LIN.Cloud.Identity.FileStorage.Abstractions;

public interface IFileStorageService
{
    Task<FileStorageResult> DownloadFileAsync(string url, CancellationToken cancellationToken = default);
    Task<FileStorageResult> GetTemporaryUrlAsync(string url, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<FileStorageResult> DeleteFileAsync(string url, CancellationToken cancellationToken = default);
}