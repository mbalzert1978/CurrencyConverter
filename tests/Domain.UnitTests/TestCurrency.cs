namespace UnitTests;

using Domain;

public class CurrencyTests
{
    [Fact]
    public void TryFromStrWhenCodeIsEmptyShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = string.Empty;

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.EmptyErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeIsNullShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = "   ";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.EmptyErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeIsWhitespaceShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = "   ";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.EmptyErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeIsTooShortShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = "AB";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.InvalidLengthErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeIsTooLongShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = "ABCD";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.InvalidLengthErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeContainsNonLetterCharactersShouldReturnDefaultCurrencyAndError()
    {
        // Arrange
        string code = "A1C";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(Currency.Default, currency);
        Assert.Equal("400", error.Code);
        Assert.Equal(Currency.InvalidCharactersErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCodeIsValidShouldReturnCurrencyAndNoError()
    {
        // Arrange
        string code = "USD";

        // Act
        Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        Assert.Equal(code, currency.Code);
        Assert.Equal(Error.None, error);
    }

    [Fact]
    public void CurrencyEqualityWhenCodesAreSameShouldBeEqual()
    {
        // Arrange
        var currency1 = new Currency("USD");
        var currency2 = new Currency("USD");

        // Act & Assert
        Assert.Equal(currency1, currency2);
    }

    [Fact]
    public void CurrencyEqualityWhenCodesAreDifferentShouldNotBeEqual()
    {
        // Arrange
        var currency1 = new Currency("USD");
        var currency2 = new Currency("EUR");

        // Act & Assert
        Assert.NotEqual(currency1, currency2);
    }
}
