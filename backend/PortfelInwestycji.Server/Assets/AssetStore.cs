using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using PortfelInwestycji.Server.Configuration;

namespace PortfelInwestycji.Server.Assets;

/// <summary>
/// Keeps the user's assets in <c>assets.json</c> in the data directory. One instance per process:
/// every read and write goes through a single lock, and every write replaces the file atomically,
/// so a crash mid-write leaves the previous version intact.
/// </summary>
public sealed class AssetStore(AppPaths paths, ILogger<AssetStore> logger)
{
    public const string FileName = "assets.json";
    public const int CurrentVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        // Keep Polish letters readable in the file rather than \u-escaped.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
    };

    private readonly SemaphoreSlim gate = new(1, 1);
    private IReadOnlyList<Asset>? assets;

    public string FilePath { get; } = Path.Combine(paths.DataDirectory, FileName);

    /// <summary>
    /// Loads the file into memory. Called once at startup so a broken file stops the app
    /// immediately instead of surfacing on the first request; the file is never overwritten then.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            assets = await ReadFileAsync(cancellationToken);
            logger.LogInformation("Loaded {Count} assets from {Path}", assets.Count, FilePath);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<IReadOnlyList<Asset>> ListAsync(CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            return assets ??= await ReadFileAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var current = assets ?? await ReadFileAsync(cancellationToken);
            IReadOnlyList<Asset> updated = [.. current, asset];
            await WriteFileAsync(updated, cancellationToken);
            // Only after the file is safely replaced does memory reflect the change.
            assets = updated;
            logger.LogInformation("Added asset {AssetId} to {Path}", asset.Id, FilePath);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<IReadOnlyList<Asset>> ReadFileAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        AssetsFile? file;
        try
        {
            await using var stream = File.OpenRead(FilePath);
            file = await JsonSerializer.DeserializeAsync<AssetsFile>(stream, JsonOptions, cancellationToken);
        }
        catch (JsonException exception)
        {
            throw Invalid($"it is not valid JSON ({exception.Message})", exception);
        }

        if (file is null)
        {
            throw Invalid("it is empty");
        }

        if (file.Version != CurrentVersion)
        {
            throw Invalid($"its version is {file.Version}, this app reads version {CurrentVersion}");
        }

        if (file.Assets is null)
        {
            throw Invalid("it has no \"assets\" list");
        }

        return file.Assets;
    }

    private async Task WriteFileAsync(IReadOnlyList<Asset> content, CancellationToken cancellationToken)
    {
        var temporaryPath = FilePath + ".tmp";
        await using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await JsonSerializer.SerializeAsync(stream, new AssetsFile(CurrentVersion, content), JsonOptions, cancellationToken);
            stream.Flush(flushToDisk: true);
        }

        File.Move(temporaryPath, FilePath, overwrite: true);
    }

    private InvalidDataException Invalid(string reason, Exception? inner = null)
    {
        var exception = new InvalidDataException(
            $"Cannot read assets file {FilePath}: {reason}. Fix the file or move it away; it has not been changed.",
            inner);
        logger.LogCritical(exception, "Cannot read assets file {Path}", FilePath);
        return exception;
    }

    private sealed record AssetsFile(int Version, IReadOnlyList<Asset>? Assets);
}
