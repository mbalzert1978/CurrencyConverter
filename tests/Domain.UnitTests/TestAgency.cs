namespace UnitTests;

using Domain;

public class AgencyTests
{
    [Fact]
    public void TryCreateWhenCurrencyCodeIsInvalidShouldReturnDefaultAgencyAndError()
    {
        // Arrange
        string name = "Test Agency";
        string address = "123 Test St";
        string country = "Testland";
        string code = "invalid";

        // Act
        Agency.TryCreate(name, address, country, code, out var agency, out var error);

        // Assert
        Assert.Equal(Agency.Default, agency);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void TryCreateWhenAllInputsAreValidShouldReturnAgencyAndNoError()
    {
        // Arrange
        string name = "Test Agency";
        string address = "123 Test St";
        string country = "Testland";
        string code = "USD";

        // Act
        Agency.TryCreate(name, address, country, code, out var agency, out var error);

        // Assert
        Assert.NotEqual(Agency.Default, agency);
        Assert.Equal(name, agency.Name);
        Assert.Equal(address, agency.Address);
        Assert.Equal(country, agency.Country);
        Assert.Equal("USD", agency.BaseCurrency.Code);
        Assert.Equal(Error.None, error);
    }

    [Fact]
    public void AddRateWhenRateIsInvalidShouldReturnError()
    {
        // Arrange
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        string currencyFrom = "invalid";
        string currencyTo = "EUR";
        string amount = "1.23";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        agency.AddRate(currencyFrom, currencyTo, amount, dateTime, out var error);

        // Assert
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void AddRateWhenAllInputsAreValidShouldAddRateAndNoError()
    {
        // Arrange
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        string currencyFrom = "USD";
        string currencyTo = "EUR";
        string amount = "1.23";
        string dateTime = "2023-10-01T00:00:00";

        // Act
        agency.AddRate(currencyFrom, currencyTo, amount, dateTime, out var error);

        // Assert
        Assert.Equal(Error.None, error);
        Assert.Single(agency.GetRates());
    }

    [Fact]
    public void GetRateWhenCurrencyFromIsInvalidShouldReturnNull()
    {
        // Arrange
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        string currencyFrom = "invalid";
        string currencyTo = "EUR";
        DateTime? dateTime = DateTime.Parse("2023-10-01T00:00:00");

        // Act
        var rate = agency.GetRate(currencyFrom, currencyTo, dateTime, out Error error);


        // Assert
        Assert.Null(rate);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void GetRateWhenCurrencyToIsInvalidShouldReturnNull()
    {
        // Arrange
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        string currencyFrom = "USD";
        string currencyTo = "invalid";
        DateTime? dateTime = DateTime.Parse("2023-10-01T00:00:00");

        // Act
        var rate = agency.GetRate(currencyFrom, currencyTo, dateTime, out Error error);

        // Assert
        Assert.Null(rate);
        Assert.NotEqual(Error.None, error);
    }

    [Fact]
    public void GetRateWhenAllInputsAreValidShouldReturnRate()
    {
        // Arrange
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        agency.AddRate("USD", "EUR", "1.23", "2023-10-01T00:00:00", out _);
        string currencyFrom = "USD";
        string currencyTo = "EUR";
        DateTime? dateTime = DateTime.Parse("2023-10-01T00:00:00");

        // Act
        var rate = agency.GetRate(currencyFrom, currencyTo, dateTime, out Error error);

        // Assert
        Assert.NotNull(rate);
        Assert.Equal(Error.None, error);
        Assert.Equal("USD", rate.CurrencyFrom.Code);
        Assert.Equal("EUR", rate.CurrencyTo.Code);
        Assert.Equal(1.23m, rate.Amount.Amount);
        Assert.Equal(dateTime, rate.DateTime);
    }
}
