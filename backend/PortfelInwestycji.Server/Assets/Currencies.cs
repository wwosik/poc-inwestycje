namespace PortfelInwestycji.Server.Assets;

public static class Currencies
{
    public const string Default = "PLN";

    /// <summary>Supported ISO 4217 codes, in the order they are offered and totalled.</summary>
    public static readonly IReadOnlyList<string> Supported = ["PLN", "EUR", "USD", "GBP", "CHF"];

    public static bool IsSupported(string? code) => code is not null && Supported.Contains(code);

    /// <summary>Position in <see cref="Supported"/>; unknown codes sort last.</summary>
    public static int OrderOf(string code)
    {
        for (var i = 0; i < Supported.Count; i++)
        {
            if (Supported[i] == code)
            {
                return i;
            }
        }

        return int.MaxValue;
    }
}
