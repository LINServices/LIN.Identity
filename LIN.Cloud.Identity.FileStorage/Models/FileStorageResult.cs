namespace LIN.Cloud.Identity.FileStorage.Models;

public sealed class FileStorageResult
{
    public bool IsSuccess { get; private init; }
    public string? Message { get; private init; }
    public Stream? Stream { get; private init; }
    public string? Url { get; private init; }

    public static FileStorageResult Success() =>
        new() { IsSuccess = true };

    public static FileStorageResult Success(Stream stream) =>
        new() { IsSuccess = true, Stream = stream };

    public static FileStorageResult SuccessUrl(string url) =>
        new() { IsSuccess = true, Url = url };

    public static FileStorageResult Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}