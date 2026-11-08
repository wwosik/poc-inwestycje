namespace PortfelInwestycji.Server.Assets;

/// <summary>Closed list of asset kinds. Persisted by name, so members may be reordered but not renamed.</summary>
public enum AssetKind
{
    Stock,
    Bond,
    Fund,
    Deposit,
    Cash,
    RealEstate,
    Crypto,
    Other,
}

public static class AssetKindLabels
{
    private static readonly IReadOnlyDictionary<AssetKind, string> Labels = new Dictionary<AssetKind, string>
    {
        [AssetKind.Stock] = "Akcje",
        [AssetKind.Bond] = "Obligacje",
        [AssetKind.Fund] = "Fundusz / ETF",
        [AssetKind.Deposit] = "Lokata",
        [AssetKind.Cash] = "Gotówka",
        [AssetKind.RealEstate] = "Nieruchomość",
        [AssetKind.Crypto] = "Kryptowaluty",
        [AssetKind.Other] = "Inne",
    };

    public static string Label(this AssetKind kind) => Labels[kind];
}
