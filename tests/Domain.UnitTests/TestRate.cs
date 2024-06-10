namespace UnitTests;

using Domain;

public class RateTests
{
    public static IEnumerable<object[]> RateTestCases =>
    [
        ["USD", "EUR", "1,23", "2023-10-01T00:00:00", false, string.Empty,string.Empty],
        ["USD", "EUR", "1,23", "invalid-date", true, StatusCode.BadRequest, Rate.ParsingErrorMessage],
        ["invalid", "EUR", "1,23", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Currency.InvalidLengthMessage],
        ["USD", "invalid", "1,23", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Currency.InvalidLengthMessage],
        ["USD", "EUR", "invalid", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Money.InvalidCharactersMessage],
        ["USD", "EUR", "invalid", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Money.InvalidCharactersMessage],
    ];
    [Theory]
    [MemberData(nameof(RateTestCases))]
    public void TryFromStrWhenInputShouldReturmExpected(
        string currencyFrom,
        string currencyTo,
        string amount,
        string dateTime,
        bool isError,
        string statusCode,
        string expectedMessage
    )
    {
        var rate = Rate.TryFromStr(currencyFrom, currencyTo, amount, dateTime, out Error error);

        if (isError)
        {
            Assert.Equal(Rate.Default, rate);
            Assert.Equal(statusCode, error.Code);
            Assert.Equal(expectedMessage, error.Description);
        }
        else
        {
            Assert.NotNull(rate);
            Assert.Equal(currencyFrom, rate.CurrencyFrom.Code);
            Assert.Equal(currencyTo, rate.CurrencyTo.Code);
            Assert.Equal(amount, rate.Amount.Amount.ToString());
            Assert.Equal(DateTime.Parse(dateTime), rate.DateTime);
            Assert.Equal(Error.None, error);
        }
    }

    public static IEnumerable<object[]> EqualityTestCases =>
    [
        ["USD", "EUR", "USD", "GBP", false],
        ["USD", "EUR", "USD", "EUR", true],
    ];

    [Theory]
    [MemberData(nameof(EqualityTestCases))]
    public void EqualityWhenEqualShouldBeTrue(string leftFrom,
                                                          string leftTo,
                                                          string rightFrom,
                                                          string rightTo,
                                                          bool equal)
    {
        var left = new Rate(
            new Currency(leftFrom),
            new Currency(leftTo),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );
        var right = new Rate(
            new Currency(rightFrom),
            new Currency(rightTo),
            new Money(1.23m),
            DateTime.Parse("2023-10-01T00:00:00")
        );
        Assert.Equal(equal, left.Equals(right));
        Assert.Equal(equal, left == right);
        Assert.Equal(!equal, left != right);
    }
}
