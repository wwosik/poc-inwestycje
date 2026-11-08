using System.ComponentModel.DataAnnotations;
using PortfelInwestycji.Server.Assets;

namespace PortfelInwestycji.Server.Tests.Assets;

public class AssetInputTests
{
    private static AssetInput Valid() => new()
    {
        Name = "Obligacje EDO",
        Kind = AssetKind.Bond,
        NominalValue = "10 000,00",
        Currency = "PLN",
    };

    private static Dictionary<string, string> Errors(AssetInput input)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(input, new ValidationContext(input), results, validateAllProperties: true);
        return results.ToDictionary(r => r.MemberNames.Single(), r => r.ErrorMessage!);
    }

    [Fact]
    public void ValidInput_HasNoErrors()
    {
        Assert.Empty(Errors(Valid()));
    }

    [Fact]
    public void EmptyInput_ReportsEveryRequiredFieldAtOnce()
    {
        var errors = Errors(new AssetInput { Currency = null });

        Assert.Equal(
            ["Currency", "Kind", "Name", "NominalValue"],
            errors.Keys.Order());
        Assert.Equal("Podaj nazwę aktywa.", errors["Name"]);
    }

    [Fact]
    public void Currency_DefaultsToPln()
    {
        Assert.Equal("PLN", new AssetInput().Currency);
    }

    [Fact]
    public void WhitespaceOnlyName_IsMissing()
    {
        var input = Valid();
        input.Name = "   ";

        Assert.Equal("Podaj nazwę aktywa.", Errors(input)["Name"]);
    }

    [Theory]
    [InlineData("-1", "Wartość nominalna nie może być ujemna.")]
    [InlineData("1,005", "Podaj kwotę z dokładnością do grosza — najwyżej 2 miejsca po przecinku.")]
    [InlineData("dużo", "Wpisz kwotę liczbą, np. 1 234,56.")]
    public void BadNominalValue_IsExplained(string value, string message)
    {
        var input = Valid();
        input.NominalValue = value;

        Assert.Equal(message, Errors(input)["NominalValue"]);
    }

    [Fact]
    public void ZeroNominalValue_IsAllowed()
    {
        var input = Valid();
        input.NominalValue = "0";

        Assert.Empty(Errors(input));
    }

    [Fact]
    public void UnsupportedCurrency_IsRejected()
    {
        var input = Valid();
        input.Currency = "JPY";

        Assert.StartsWith("Wybierz walutę z listy", Errors(input)["Currency"]);
    }

    [Fact]
    public void UndefinedKind_IsRejected()
    {
        var input = Valid();
        input.Kind = (AssetKind)99;

        Assert.Equal("Wybierz rodzaj aktywa z listy.", Errors(input)["Kind"]);
    }

    [Fact]
    public void TooLongFields_AreRejectedTogether()
    {
        var input = Valid();
        input.Name = new string('a', AssetInput.NameMaxLength + 1);
        input.Identifier = new string('A', AssetInput.IdentifierMaxLength + 1);
        input.Note = new string('n', AssetInput.NoteMaxLength + 1);

        Assert.Equal(["Identifier", "Name", "Note"], Errors(input).Keys.Order());
    }

    [Fact]
    public void TextFields_AreTrimmed_AndBlankOptionalsBecomeNull()
    {
        var input = new AssetInput { Name = "  CD Projekt  ", Identifier = "  ", Note = " notatka " };

        Assert.Equal("CD Projekt", input.Name);
        Assert.Null(input.Identifier);
        Assert.Equal("notatka", input.Note);
    }
}
