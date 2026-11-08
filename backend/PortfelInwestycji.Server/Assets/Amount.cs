using System.Globalization;

namespace PortfelInwestycji.Server.Assets;

/// <summary>Parses and formats money amounts the way a Polish user writes them.</summary>
public static class Amount
{
    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pl-PL");

    // pl-PL groups thousands with U+00A0; people also type a plain space or U+202F.
    private static readonly char[] GroupSeparators = [' ', ' ', ' '];

    /// <summary>
    /// Accepts "1234,56", "1 234,56" and, when there is no comma, "1234.56".
    /// </summary>
    public static bool TryParse(string? text, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalized = string.Concat(text.Trim().Split(GroupSeparators, StringSplitOptions.RemoveEmptyEntries));
        if (!normalized.Contains(',') && normalized.Count(c => c == '.') == 1)
        {
            normalized = normalized.Replace('.', ',');
        }

        return decimal.TryParse(
            normalized,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            Culture,
            out value);
    }

    public static bool HasAtMostTwoDecimals(decimal value) => decimal.Round(value, 2) == value;

    public static string Format(decimal value) => value.ToString("N2", Culture);

    public static string Format(decimal value, string currency) => $"{Format(value)} {currency}";
}
