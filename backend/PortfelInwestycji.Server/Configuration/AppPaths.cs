namespace PortfelInwestycji.Server.Configuration;

/// <summary>
/// Resolved, absolute paths for the app's persistent state.
/// The defaults in appsettings.json resolve relative to the repo root for local development;
/// a container overrides both with environment variables pointing at its mounted volumes.
/// </summary>
public sealed record AppPaths(string DataDirectory, string LogsDirectory)
{
    public static AppPaths Resolve(IConfiguration configuration, string contentRootPath)
    {
        var dataDirectory = ResolveDirectory(configuration["AppPaths:DataDirectory"], contentRootPath);
        var logsDirectory = ResolveDirectory(configuration["AppPaths:LogsDirectory"], contentRootPath);

        Directory.CreateDirectory(dataDirectory);
        Directory.CreateDirectory(logsDirectory);

        return new AppPaths(dataDirectory, logsDirectory);
    }

    private static string ResolveDirectory(string? configuredPath, string contentRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configuredPath);

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(contentRootPath, configuredPath));
    }
}
