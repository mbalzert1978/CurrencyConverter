namespace Domain.UnitTests;

using Domain;

public class RateTests
{
    public static IEnumerable<object[]> ValidRateData =>
    [
        ["USD", "EUR", "0.85", DateTime.Parse("2023-10-01T00:00:00"), ],
        ["USD", "EUR", "1.23456789", DateTime.Parse("2023-10-01T00:00:00")],
        ["USD", "EUR", "1.234567880", DateTime.Parse("2023-10-01T00:00:00")],
        ["USD", "EUR", "1.234567886", DateTime.Parse("2023-10-01T00:00:00")],
    ];

    [Theory]
    [MemberData(nameof(ValidRateData))]
    public void CreateWhenAllParametersAreValidShouldReturnRateInstance(string expectedCurrencyFrom,
                                                                        string expectedCurrencyTo,
                                                                        string expectedRate,
                                                                        DateTime expectedDate)
    {
        // Act
        var rateInstance = Rate.FromStr(expectedCurrencyFrom, expectedCurrencyTo, expectedRate, expectedDate.ToString());

        // Assert
        Assert.Equal(Currency.FromStr(expectedCurrencyFrom), rateInstance.GetAtomicValues().First());
        Assert.Equal(Currency.FromStr(expectedCurrencyTo), rateInstance.GetAtomicValues().Skip(1).First());
        Assert.Equal(Money.FromStr(expectedRate), rateInstance.GetAtomicValues().Skip(2).First());
        Assert.Equal(expectedDate, rateInstance.GetAtomicValues().Skip(3).First());
    }

    public static IEnumerable<object[]> InvalidRateData =>
    [
        ["US", "EUR", "0.85", "2023-10-01T00:00:00", "Currency code must be exactly 3 characters long."],
        ["USD", "EUR", "invalid_rate", "2023-10-01T00:00:00", "Money amount can only contain numbers and/or a dot."],
        ["USD", "EUR", "0.85", "invalid_date", "DateTime is not in a valid ISO 8601 format."],
        ["USD", "EUR", "0.00", "2023-10-01T00:00:00", "Money amount cannot be negative."],
        ["USD", "EUR", "-0.85", "2023-10-01T00:00:00", "Money amount can only contain numbers and/or a dot."],
        ["USDE", "EUR", "0.85", "2023-10-01T00:00:00", "Currency code must be exactly 3 characters long."],
        ["USD", "EUR", "0.85", "", "DateTime is not in a valid ISO 8601 format."],
    ];

    [Theory]
    [MemberData(nameof(InvalidRateData))]
    public void CreateWhenInvalidParametersShouldReturnExpectedErrors(
        string currencyFrom, string currencyTo, string rate, string date, string expectedErrorMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<Error>(() => Rate.FromStr(currencyFrom, currencyTo, rate, date));
        Assert.Equal(expectedErrorMessage, exception.Description);
    }
    public static IEnumerable<object[]> RateTestData =>
    [
        ["2", "0.5", "1"],
        ["4", "2", "8"],
        ["1.00000000", "0.25", "0.25"],
    ];

    [Theory]
    [MemberData(nameof(RateTestData))]
    public void RateWhenMultiplyShouldReturnExpectedResults(string left, string right, string expected)
    {
        Assert.Equal(Rate.FromStr("USD", "EUR", left, "2023-10-01T00:00:00")
                         .Multiply(Rate.FromStr("JPY", "USD", right, "2023-10-01T00:00:00")),
                            Rate.FromStr("JPY", "EUR", expected, "2023-10-01T00:00:00"));
    }

    [Fact]
    public void RateWhenInvertRateShouldReturnInvertedRate() => Assert.Equal(Rate.FromStr("USD", "EUR", "2", "2023-10-01T00:00:00")
                                                                                 .Invert(), Rate.FromStr("EUR",
                                                                                                         "USD",
                                                                                                         "0.50000000",
                                                                                                         "2023-10-01T00:00:00"));

}
