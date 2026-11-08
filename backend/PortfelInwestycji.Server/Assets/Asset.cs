namespace PortfelInwestycji.Server.Assets;

public sealed record Asset
{
    public Guid Id { get; init; }

    public string Name { get; init; } = "";

    public AssetKind Kind { get; init; }

    /// <summary>The value the user declares for the asset; nothing is derived from it.</summary>
    public decimal NominalValue { get; init; }

    /// <summary>ISO 4217 code, one of <see cref="Currencies.Supported"/>.</summary>
    public string Currency { get; init; } = Currencies.Default;

    /// <summary>Optional ticker or ISIN, upper-case.</summary>
    public string? Identifier { get; init; }

    public string? Note { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
