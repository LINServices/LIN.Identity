#r "nuget: FluentFTP, 53.0.1"
#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

var options = new FtpCleanerOptions
{
    Host = Environment.GetEnvironmentVariable("FTP_HOST") ?? "",
    Port = 21,
    User = Environment.GetEnvironmentVariable("FTP_USER") ?? "",
    Pass = Environment.GetEnvironmentVariable("FTP_PASS") ?? "",
    RemoteDir = Environment.GetEnvironmentVariable("FTP_DIR") ?? "/",
    MaxTries = 5
};

var progress = new Progress<string>(Console.WriteLine);
var cts = new CancellationTokenSource();

await FtpCleaner.CleanAsync(options, progress, cts.Token);


// ====================== IMPLEMENTACIÓN ======================

public sealed class FtpCleanerOptions
{
    public string Host { get; init; } = default!;
    public int Port { get; init; } = 21;
    public string User { get; init; } = default!;
    public string Pass { get; init; } = default!;
    public string RemoteDir { get; init; } = "/";
    public int MaxTries { get; init; } = 5;
}

public static class FtpCleaner
{
    public static async Task CleanAsync(
        FtpCleanerOptions opt,
        IProgress<string>? log,
        CancellationToken ct)
    {
        using var client = CreateClient(opt);

        client.Connect();

        var baseDir = NormalizeDir(opt.RemoteDir);

        for (int attempt = 1; attempt <= opt.MaxTries; attempt++)
        {
            log?.Report($"🧹 Limpieza FTP intento {attempt}/{opt.MaxTries}...");

            var items = client.GetListing(baseDir, FtpListOption.ForceList);

            var keepFiles = items
                .Where(i => i.Type == FtpObjectType.File &&
                            i.Name.StartsWith("appsettings.", StringComparison.OrdinalIgnoreCase))
                .Select(i => NormalizePath(i.FullName))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 1️⃣ Borrar archivos
            foreach (var item in items.Where(i => i.Type == FtpObjectType.File))
            {
                ct.ThrowIfCancellationRequested();

                var full = NormalizePath(item.FullName);
                if (keepFiles.Contains(full))
                    continue;

                try
                {
                    log?.Report($"  rm {full}");
                    client.DeleteFile(full);
                }
                catch (Exception ex)
                {
                    log?.Report($"  ⚠️ {full}: {ex.Message}");
                }
            }

            // 2️⃣ Borrar directorios vacíos (profundidad primero)
            var dirs = items
                .Where(i => i.Type == FtpObjectType.Directory)
                .Select(i => NormalizeDir(i.FullName))
                .OrderByDescending(d => d.Count(c => c == '/'))
                .ToList();

            foreach (var dir in dirs)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var children = client.GetListing(dir, FtpListOption.ForceList);
                    if (children.Length == 0)
                    {
                        log?.Report($"  rmdir {dir}/");
                        client.DeleteDirectory(dir);
                    }
                }
                catch (Exception ex)
                {
                    log?.Report($"  ⚠️ {dir}: {ex.Message}");
                }
            }

            // 3️⃣ Comprobar si quedó algo borrable
            var after = client.GetListing(baseDir, FtpListOption.ForceList);
            var remaining = after.Any(i =>
                i.Type == FtpObjectType.Directory ||
                (i.Type == FtpObjectType.File &&
                 !i.Name.StartsWith("appsettings.", StringComparison.OrdinalIgnoreCase)));

            if (!remaining)
            {
                log?.Report("✅ Limpieza completa (solo appsettings.json conservado).");
                break;
            }

            if (attempt == opt.MaxTries)
                throw new Exception("❌ No se pudo limpiar el FTP tras varios intentos.");

            await Task.Delay(4000, ct);
        }

        client.Disconnect();
    }

    private static FtpClient CreateClient(FtpCleanerOptions opt)
    {
        return new FtpClient(opt.Host, opt.Port)
        {
            Credentials = new NetworkCredential(opt.User, opt.Pass),
            Config =
            {
                DataConnectionType = FtpDataConnectionType.PASV,
                ConnectTimeout = 15000,
                ReadTimeout = 30000,
                DataConnectionConnectTimeout = 30000,
                DataConnectionReadTimeout = 30000,
                RetryAttempts = 2,
                SocketKeepAlive = true,
                NoopInterval = 10000
            }
        };
    }

    private static string NormalizePath(string p)
    {
        p = p.Replace('\\', '/');
        while (p.Contains("//")) p = p.Replace("//", "/");
        return p;
    }

    private static string NormalizeDir(string p)
    {
        p = NormalizePath(p);
        if (p.Length > 1 && p.EndsWith("/")) p = p.TrimEnd('/');
        return p == "" ? "/" : p;
    }
}