namespace UnitTests;

using Domain;

public class RateTests
{
    [Fact]
    public void TryFromStrWhenDateTimeIsInvalidShouldReturnDefaultRateAndError()
    {
        // Arrange
        string currencyFrom = "USD";
        string currencyTo = "EUR";
        string amount = "1.23";
        string dateTime = "invalid-date";

        // Act
        Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out var rate, out var error);

        // Assert
        Assert.Equal(Rate.Default, rate);
        Assert.Equal("400", error.Code);
        Assert.Equal(Rate.ParsingErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenCurrencyFromIsInvalidShouldReturnDefaultRateAndError()
    {
        // Arrange
        string currencyFrom = "invalid";
        string currencyTo = "EUR";
        string amount = "1.23";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out var rate, out var error);

        // Assert
        Assert.Equal(Rate.Default, rate);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void TryFromStrWhenCurrencyToIsInvalidShouldReturnDefaultRateAndError()
    {
        // Arrange
        string currencyFrom = "USD";
        string currencyTo = "invalid";
        string amount = "1.23";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out var rate, out var error);

        // Assert
        Assert.Equal(Rate.Default, rate);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void TryFromStrWhenAmountIsInvalidShouldReturnDefaultRateAndError()
    {
        // Arrange
        string currencyFrom = "USD";
        string currencyTo = "EUR";
        string amount = "invalid";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out var rate, out var error);

        // Assert
        Assert.Equal(Rate.Default, rate);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void TryFromStrWhenAllInputsAreValidShouldReturnRateAndNoError()
    {
        // Arrange
        string currencyFrom = "USD";
        string currencyTo = "EUR";
        string amount = "1.23";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out var rate, out var error);

        // Assert
        Assert.Equal("USD", rate.CurrencyFrom.Code);
        Assert.Equal("EUR", rate.CurrencyTo.Code);
        Assert.Equal(1.23m, rate.Amount.Amount);
        Assert.Equal(DateTime.Parse(dateTime), rate.DateTime);
        Assert.Equal(Error.None, error);
    }

    [Fact]
    public void RateEqualityWhenRatesAreSameShouldBeEqual()
    {
        // Arrange
        var rate1 = new Rate(
            new Currency("USD"),
            new Currency("EUR"),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );
        var rate2 = new Rate(
            new Currency("USD"),
            new Currency("EUR"),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );

        // Act & Assert
        Assert.Equal(rate1, rate2);
    }

    [Fact]
    public void RateEqualityWhenRatesAreDifferentShouldNotBeEqual()
    {
        // Arrange
        var rate1 = new Rate(
            new Currency("USD"),
            new Currency("EUR"),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );
        var rate2 = new Rate(
            new Currency("USD"),
            new Currency("JPY"),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );

        // Act & Assert
        Assert.NotEqual(rate1, rate2);
    }
}
