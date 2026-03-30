using LIN.Cloud.Identity.FileStorage.Abstractions;
using LIN.Cloud.Identity.FileStorage.Models;
using LIN.Cloud.Identity.FileStorage.Options;
using Minio;
using Minio.DataModel.Args;

namespace LIN.Cloud.Identity.FileStorage.Implementations;

internal sealed class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _client;

    public MinioFileStorageService(IOptions<FileStorageOptions> options)
    {
        var opt = options.Value;
        var (endpoint, useSsl) = ResolveEndpoint(opt.Endpoint, opt.UseSsl);

        _client = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(opt.AccessKey, opt.SecretKey)
            .WithSSL(useSsl)
            .Build();
    }

    public async Task<FileStorageResult> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var (bucketName, objectName) = ParseUrl(url);
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName))
                return FileStorageResult.Failure("No se pudo extraer el bucket y objeto de la URL proporcionada.");

            var memoryStream = new MemoryStream();
            await _client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream, ct) =>
                    {
                        await stream.CopyToAsync(memoryStream, ct);
                    }),
                cancellationToken);

            memoryStream.Position = 0;
            return FileStorageResult.Success(memoryStream);
        }
        catch (Exception ex)
        {
            return FileStorageResult.Failure($"No se pudo descargar el archivo desde MinIO: {ex.Message}");
        }
    }

    public async Task<FileStorageResult> GetTemporaryUrlAsync(string url, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        try
        {
            var (bucketName, objectName) = ParseUrl(url);
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName))
                return FileStorageResult.Failure("No se pudo extraer el bucket y objeto de la URL proporcionada.");

            var expirySeconds = (int)Math.Clamp(expiry.TotalSeconds, 1, 604800);

            var presignedUrl = await _client.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(expirySeconds));

            return FileStorageResult.SuccessUrl(presignedUrl);
        }
        catch (Exception ex)
        {
            return FileStorageResult.Failure($"No se pudo generar la URL temporal desde MinIO: {ex.Message}");
        }
    }

    public async Task<FileStorageResult> DeleteFileAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var (bucketName, objectName) = ParseUrl(url);
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName))
                return FileStorageResult.Failure("No se pudo extraer el bucket y objeto de la URL proporcionada.");

            await _client.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName),
                cancellationToken);

            return FileStorageResult.Success();
        }
        catch (Exception ex)
        {
            return FileStorageResult.Failure($"No se pudo eliminar el archivo desde MinIO: {ex.Message}");
        }
    }

    private static (string endpoint, bool useSsl) ResolveEndpoint(string configuredEndpoint, bool configuredUseSsl)
    {
        if (Uri.TryCreate(configuredEndpoint, UriKind.Absolute, out var uri))
        {
            var endpoint = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
            var useSsl = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) || configuredUseSsl;
            return (endpoint, useSsl);
        }

        return (configuredEndpoint, configuredUseSsl);
    }

    private static (string bucketName, string objectName) ParseUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var pathParts = uri.AbsolutePath.TrimStart('/').Split('/', 2);
            return pathParts.Length == 2
                ? (pathParts[0], Uri.UnescapeDataString(pathParts[1]))
                : (string.Empty, string.Empty);
        }
        catch
        {
            return (string.Empty, string.Empty);
        }
    }
}