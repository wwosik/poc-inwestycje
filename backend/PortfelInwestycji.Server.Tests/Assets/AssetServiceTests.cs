using Microsoft.Extensions.Logging.Abstractions;
using PortfelInwestycji.Server.Assets;
using PortfelInwestycji.Server.Configuration;

namespace PortfelInwestycji.Server.Tests.Assets;

public sealed class AssetServiceTests : IDisposable
{
    private static readonly DateTimeOffset Now = new(2026, 11, 8, 12, 30, 0, TimeSpan.Zero);

    private readonly TestDirectory directory = new();
    private readonly AssetService service;

    public AssetServiceTests()
    {
        var store = new AssetStore(new AppPaths(directory.Path, directory.Path), NullLogger<AssetStore>.Instance);
        service = new AssetService(store, new FixedTimeProvider(Now));
    }

    public void Dispose() => directory.Dispose();

    private Task<Asset> Add(string name, string value = "100", string currency = "PLN", string? identifier = null) =>
        service.AddAsync(new AssetInput
        {
            Name = name,
            Kind = AssetKind.Stock,
            NominalValue = value,
            Currency = currency,
            Identifier = identifier,
        });

    [Fact]
    public async Task Add_ParsesAmount_UppercasesIdentifier_AndStampsTime()
    {
        var asset = await Add("CD Projekt", "1 234,56", identifier: "cdr");

        Assert.Equal(1234.56m, asset.NominalValue);
        Assert.Equal("CDR", asset.Identifier);
        Assert.Equal(Now, asset.CreatedAt);
        Assert.NotEqual(Guid.Empty, asset.Id);
    }

    [Fact]
    public async Task Add_RefusesUnvalidatedInput()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(new AssetInput()));
    }

    [Fact]
    public async Task List_SortsByNameInPolishOrder()
    {
        foreach (var name in new[] { "Złoto", "łódź", "Ćma", "cebula", "Lokata" })
        {
            await Add(name);
        }

        var names = (await service.ListAsync()).Select(a => a.Name);

        Assert.Equal(["cebula", "Ćma", "Lokata", "łódź", "Złoto"], names);
    }

    [Fact]
    public void TotalsByCurrency_SumsEachCurrencySeparately_InSupportedOrder()
    {
        Asset Of(decimal value, string currency) => new() { NominalValue = value, Currency = currency };

        var totals = AssetService.TotalsByCurrency(
        [
            Of(100.10m, "USD"),
            Of(1000m, "PLN"),
            Of(0.90m, "USD"),
            Of(234.56m, "PLN"),
            Of(50m, "EUR"),
        ]);

        Assert.Equal(
            [new CurrencyTotal("PLN", 1234.56m), new CurrencyTotal("EUR", 50m), new CurrencyTotal("USD", 101.00m)],
            totals);
    }

    [Fact]
    public void TotalsByCurrency_OfNothing_IsEmpty()
    {
        Assert.Empty(AssetService.TotalsByCurrency([]));
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
