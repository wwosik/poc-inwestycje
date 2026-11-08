using Microsoft.Extensions.Configuration;
using PortfelInwestycji.Server.Configuration;

namespace PortfelInwestycji.Server.Tests;

public class AppPathsTests
{
    [Fact]
    public void Resolve_TurnsRelativeConfiguredPaths_IntoAbsoluteOnes()
    {
        var contentRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppPaths:DataDirectory"] = "data",
                ["AppPaths:LogsDirectory"] = "logs",
            })
            .Build();

        try
        {
            var paths = AppPaths.Resolve(configuration, contentRoot);

            Assert.Equal(Path.Combine(contentRoot, "data"), paths.DataDirectory);
            Assert.Equal(Path.Combine(contentRoot, "logs"), paths.LogsDirectory);
            Assert.True(Directory.Exists(paths.DataDirectory));
            Assert.True(Directory.Exists(paths.LogsDirectory));
        }
        finally
        {
            if (Directory.Exists(contentRoot))
            {
                Directory.Delete(contentRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void Resolve_Throws_WhenAPathIsNotConfigured()
    {
        var configuration = new ConfigurationBuilder().Build();

        Assert.Throws<ArgumentNullException>(
            () => AppPaths.Resolve(configuration, Path.GetTempPath()));
    }
}
