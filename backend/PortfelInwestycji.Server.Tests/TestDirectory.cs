namespace PortfelInwestycji.Server.Tests;

/// <summary>A fresh directory under the repo's git-ignored <c>.tmp/</c>, deleted on dispose.</summary>
public sealed class TestDirectory : IDisposable
{
    public TestDirectory()
    {
        Path = System.IO.Path.Combine(FindRepoRoot(), ".tmp", "tests", System.IO.Path.GetRandomFileName());
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, recursive: true);
        }
    }

    private static string FindRepoRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(System.IO.Path.Combine(directory.FullName, "AGENTS.md")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Repository root (with AGENTS.md) not found above " + AppContext.BaseDirectory);
    }
}
