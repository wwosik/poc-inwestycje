using System.ComponentModel.DataAnnotations;

namespace PortfelInwestycji.Server.Assets;

/// <summary>
/// What the user submits to add an asset. Every property is optional at the type level so that a
/// missing field is a validation error, reported together with all the others, not a binding failure.
/// </summary>
public sealed class AssetInput
{
    public const int NameMaxLength = 200;
    public const int IdentifierMaxLength = 32;
    public const int NoteMaxLength = 1000;

    [Display(Name = "Nazwa")]
    [Required(ErrorMessage = "Podaj nazwę aktywa.")]
    [StringLength(NameMaxLength, ErrorMessage = "Nazwa może mieć najwyżej {1} znaków.")]
    public string? Name { get; set => field = value?.Trim(); }

    [Display(Name = "Rodzaj")]
    [Required(ErrorMessage = "Wybierz rodzaj aktywa.")]
    [EnumDataType(typeof(AssetKind), ErrorMessage = "Wybierz rodzaj aktywa z listy.")]
    public AssetKind? Kind { get; set; }

    /// <summary>Kept as text: the user types a Polish-formatted amount, parsed by <see cref="Amount"/>.</summary>
    [Display(Name = "Wartość nominalna")]
    [Required(ErrorMessage = "Podaj wartość nominalną.")]
    [NominalValue]
    public string? NominalValue { get; set; }

    [Display(Name = "Waluta")]
    [Required(ErrorMessage = "Wybierz walutę.")]
    [SupportedCurrency]
    public string? Currency { get; set; } = Currencies.Default;

    [Display(Name = "Ticker / ISIN")]
    [StringLength(IdentifierMaxLength, ErrorMessage = "Ticker / ISIN może mieć najwyżej {1} znaki.")]
    public string? Identifier { get; set => field = NullIfEmpty(value); }

    [Display(Name = "Notatka")]
    [StringLength(NoteMaxLength, ErrorMessage = "Notatka może mieć najwyżej {1} znaków.")]
    public string? Note { get; set => field = NullIfEmpty(value); }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class NominalValueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // An empty value is [Required]'s to report.
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Success;
        }

        var members = validationContext.MemberName is { } member ? new[] { member } : null;

        if (!Amount.TryParse(text, out var amount))
        {
            return new ValidationResult("Wpisz kwotę liczbą, np. 1 234,56.", members);
        }

        if (amount < 0)
        {
            return new ValidationResult("Wartość nominalna nie może być ujemna.", members);
        }

        if (!Amount.HasAtMostTwoDecimals(amount))
        {
            return new ValidationResult("Podaj kwotę z dokładnością do grosza — najwyżej 2 miejsca po przecinku.", members);
        }

        return ValidationResult.Success;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class SupportedCurrencyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || Currencies.IsSupported(value as string))
        {
            return ValidationResult.Success;
        }

        var members = validationContext.MemberName is { } member ? new[] { member } : null;
        return new ValidationResult(
            $"Wybierz walutę z listy: {string.Join(", ", Currencies.Supported)}.", members);
    }
}
