namespace PortfelInwestycji.Server.Assets;

public sealed record CurrencyTotal(string Currency, decimal Total);

public sealed class AssetService(AssetStore store, TimeProvider timeProvider)
{
    private static readonly StringComparer NameComparer = StringComparer.Create(Amount.Culture, ignoreCase: true);

    /// <summary>All assets, sorted by name the way a Polish reader expects (Ć after C, Ł after L).</summary>
    public async Task<IReadOnlyList<Asset>> ListAsync(CancellationToken cancellationToken = default)
    {
        var assets = await store.ListAsync(cancellationToken);
        return [.. assets.OrderBy(a => a.Name, NameComparer).ThenBy(a => a.CreatedAt)];
    }

    /// <summary>Adds an asset from input that has already passed validation.</summary>
    public async Task<Asset> AddAsync(AssetInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(input.Name)
            || input.Kind is not { } kind
            || !Amount.TryParse(input.NominalValue, out var nominalValue)
            || !Currencies.IsSupported(input.Currency))
        {
            throw new ArgumentException("Asset input must be validated before it is added.", nameof(input));
        }

        var asset = new Asset
        {
            Id = Guid.CreateVersion7(),
            Name = input.Name,
            Kind = kind,
            NominalValue = nominalValue,
            Currency = input.Currency!,
            Identifier = input.Identifier?.ToUpperInvariant(),
            Note = input.Note,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        await store.AddAsync(asset, cancellationToken);
        return asset;
    }

    /// <summary>Sum of nominal values for each currency present, without any exchange-rate conversion.</summary>
    public static IReadOnlyList<CurrencyTotal> TotalsByCurrency(IEnumerable<Asset> assets) =>
    [
        .. assets
            .GroupBy(a => a.Currency)
            .Select(g => new CurrencyTotal(g.Key, g.Sum(a => a.NominalValue)))
            .OrderBy(t => Currencies.OrderOf(t.Currency))
            .ThenBy(t => t.Currency, StringComparer.Ordinal),
    ];
}
