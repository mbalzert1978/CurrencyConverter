namespace UnitTests;

using Domain;

public class CurrencyTests
{
#pragma warning disable CS8625, CS8602, CS8604
    public static IEnumerable<object[]> CurrencyTestCases =>
    [
        ["USD", "USD", string.Empty, string.Empty],
        ["EUR", "EUR", string.Empty, string.Empty],
        ["US", "DEFAULT", StatusCode.BadRequest, "Currency code must be exactly 3 characters long."],
        ["U", "DEFAULT", StatusCode.BadRequest, "Currency code must be exactly 3 characters long."],
        ["USDE", "DEFAULT", StatusCode.BadRequest, "Currency code must be exactly 3 characters long."],
        ["123", "DEFAULT", StatusCode.BadRequest, "Currency code can only contain letters."],
        ["+US", "DEFAULT", StatusCode.BadRequest, "Currency code can only contain letters."],
        ["", "DEFAULT", StatusCode.BadRequest, "Currency code cannot be empty."],
        [null, "DEFAULT", StatusCode.BadRequest, "Currency code cannot be empty."],
    ];

    [Theory]
    [MemberData(nameof(CurrencyTestCases))]
    public void CurrencyTryFromStrWhenInputShouldReturnExpectedOutput(string? code,
                                                            string expectedCode,
                                                            string statusCode,
                                                            string expectedMessage)
    {
        Currency.TryFromStr(code, out var currency, out var error);
        switch (expectedCode)
        {
            case "DEFAULT":
                Assert.Equal(Currency.Default, currency);
                Assert.Equal(statusCode, error.Code);
                Assert.Equal(expectedMessage, error.Description);
                break;
            default:
                Assert.Equal(expectedCode, currency.Code);
                Assert.Equal(Error.None, error);
                break;
        }
    }

    public static IEnumerable<object[]> CurrencyEqualityCases =>
    [
        ["USD", "USD", true],
        ["USD", "GBP", false]
    ];

    [Theory]
    [MemberData(nameof(CurrencyEqualityCases))]
    public void EqualityWhenEqualShouldBeTrue(string left,
                                              string right,
                                              bool expectedEquality)
    {
        Currency currencyLeft = new(left);
        Currency currencyRight = new(right);
        Assert.Equal(expectedEquality, currencyLeft.Equals(currencyRight));
        Assert.Equal(expectedEquality, currencyLeft == currencyRight);
        Assert.Equal(!expectedEquality, currencyLeft != currencyRight);
    }
#pragma warning restore CS8602, CS8625

}
