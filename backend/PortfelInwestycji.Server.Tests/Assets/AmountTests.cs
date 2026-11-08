using PortfelInwestycji.Server.Assets;

namespace PortfelInwestycji.Server.Tests.Assets;

public class AmountTests
{
    [Theory]
    [InlineData("1234,56", "1234.56")]
    [InlineData("1 234,56", "1234.56")]
    [InlineData("1 234,56", "1234.56")]
    [InlineData("1 234,56", "1234.56")]
    [InlineData(" 10 000 ", "10000")]
    [InlineData("1234.56", "1234.56")]
    [InlineData("0", "0")]
    [InlineData("-5,00", "-5.00")]
    public void TryParse_AcceptsPolishWrittenAmounts(string text, string expected)
    {
        Assert.True(Amount.TryParse(text, out var value));
        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture), value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("1,2,3")]
    [InlineData("1.234,56.7")]
    [InlineData("12zł")]
    [InlineData("1e5")]
    public void TryParse_RejectsWhatIsNotAnAmount(string? text)
    {
        Assert.False(Amount.TryParse(text, out _));
    }

    [Fact]
    public void Format_UsesPolishGroupingAndComma()
    {
        Assert.Equal("1 234 567,50 PLN", Amount.Format(1234567.5m, "PLN"));
    }

    [Theory]
    [InlineData("1.5", true)]
    [InlineData("1.50", true)]
    [InlineData("1.500", true)]
    [InlineData("1.505", false)]
    public void HasAtMostTwoDecimals_IgnoresTrailingZeros(string value, bool expected)
    {
        Assert.Equal(expected, Amount.HasAtMostTwoDecimals(decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture)));
    }
}
