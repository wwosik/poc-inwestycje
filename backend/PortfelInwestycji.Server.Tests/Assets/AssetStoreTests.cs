using Microsoft.Extensions.Logging.Abstractions;
using PortfelInwestycji.Server.Assets;
using PortfelInwestycji.Server.Configuration;

namespace PortfelInwestycji.Server.Tests.Assets;

public sealed class AssetStoreTests : IDisposable
{
    private readonly TestDirectory directory = new();

    public void Dispose() => directory.Dispose();

    private AssetStore NewStore() =>
        new(new AppPaths(directory.Path, directory.Path), NullLogger<AssetStore>.Instance);

    private static Asset SampleAsset(string name = "Obligacje EDO") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Kind = AssetKind.Bond,
        NominalValue = 10000.00m,
        Currency = "PLN",
        Identifier = "EDO0934",
        Note = "Zakup w PKO",
        CreatedAt = new DateTimeOffset(2026, 11, 8, 10, 0, 0, TimeSpan.Zero),
    };

    [Fact]
    public async Task MissingFile_MeansNoAssets()
    {
        var store = NewStore();
        await store.LoadAsync();

        Assert.Empty(await store.ListAsync());
        Assert.False(File.Exists(store.FilePath));
    }

    [Fact]
    public async Task AddedAsset_SurvivesANewStoreInstance()
    {
        var asset = SampleAsset("Złoto łódź");
        await NewStore().AddAsync(asset);

        var reopened = NewStore();
        await reopened.LoadAsync();

        var loaded = Assert.Single(await reopened.ListAsync());
        Assert.Equal(asset, loaded);
        Assert.Equal(2, decimal.GetBits(loaded.NominalValue)[3] >> 16 & 0xFF); // scale kept: 10000.00
    }

    [Fact]
    public async Task File_IsVersionedReadableJson_WithKindByName()
    {
        var store = NewStore();
        await store.AddAsync(SampleAsset("Złoto"));

        var json = await File.ReadAllTextAsync(store.FilePath);

        Assert.Contains("\"version\": 1", json);
        Assert.Contains("\"kind\": \"Bond\"", json);
        Assert.Contains("\"nominalValue\": 10000.00", json);
        Assert.Contains("Złoto", json);
        Assert.False(File.Exists(store.FilePath + ".tmp"));
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("null")]
    [InlineData("""{ "version": 2, "assets": [] }""")]
    [InlineData("""{ "version": 1 }""")]
    [InlineData("""{ "version": 1, "assets": [ { "kind": "Spaceship" } ] }""")]
    public async Task UnreadableFile_FailsLoad_AndIsLeftUntouched(string content)
    {
        var store = NewStore();
        await File.WriteAllTextAsync(store.FilePath, content);

        var exception = await Assert.ThrowsAsync<InvalidDataException>(() => store.LoadAsync());

        Assert.Contains(store.FilePath, exception.Message);
        Assert.Equal(content, await File.ReadAllTextAsync(store.FilePath));
    }

    [Fact]
    public async Task ConcurrentAdds_AreAllKept()
    {
        var store = NewStore();
        await store.LoadAsync();

        await Task.WhenAll(Enumerable.Range(0, 25).Select(i => store.AddAsync(SampleAsset($"Aktywo {i}"))));

        var reopened = NewStore();
        await reopened.LoadAsync();
        Assert.Equal(25, (await reopened.ListAsync()).Count);
    }
}
