namespace UnitTests;

using Domain;

public class MoneyTests
{
#pragma warning disable CS8625, CS8602, CS8604

    public static IEnumerable<object[]> MoneyTestCases =>
    [
        [string.Empty, "DEFAULT", StatusCode.BadRequest, Money.EmptyMessage],
        [null, "DEFAULT", StatusCode.BadRequest, Money.EmptyMessage],
        ["   ", "DEFAULT", StatusCode.BadRequest, Money.EmptyMessage],
        ["12.34a", "DEFAULT", StatusCode.BadRequest, Money.InvalidCharactersMessage],
        ["-12.34", "DEFAULT", StatusCode.BadRequest, Money.InvalidCharactersMessage],
        ["12.34.56", "DEFAULT", StatusCode.BadRequest, Money.ParsingErrorMessage],
        ["12", "12", string.Empty, string.Empty],
    ];
    [Theory]
    [MemberData(nameof(MoneyTestCases))]
    public void TryFromStrWhenGivenAStringShouldReturnExpectedResults(string? amount, string expectedMoney, string statusCode, string expectedMessage)
    {
        Money.TryFromStr(amount, out var money, out var error);
        switch (expectedMoney)
        {
            case "DEFAULT":
                Assert.Equal(Money.Default, money);
                Assert.Equal(statusCode, error.Code);
                Assert.Equal(expectedMessage, error.Description);
                break;
            default:
                Assert.Equal(Math.Round(decimal.Parse(expectedMoney), 8), money.Amount);
                Assert.Equal(Error.None, error);
                break;
        }
    }


    [Fact]
    public void TryFromDecimalWhenAmountIsNegativeShouldReturnDefaultMoneyAndError()
    {
        Money.TryFromDecimal(-12.34m, out var money, out var error);

        Assert.Equal(Money.Default, money);
        Assert.Equal(StatusCode.BadRequest, error.Code);
        Assert.Equal(Money.NotNegativeMessage, error.Description);
    }

    [Fact]
    public void TryFromDecimalWhenAmountIsValidShouldReturnMoneyAndNoError()
    {
        Money.TryFromDecimal(12.34m, out var money, out var error);

        Assert.Equal(12.34m, money.Amount);
        Assert.Equal(Error.None, error);
    }

    public static IEnumerable<object[]> CurrencyEqualityCases =>
        [
            [12, 12, true],
            [12.5, 5.125, false]
        ];

    [Theory]
    [MemberData(nameof(CurrencyEqualityCases))]
    public void EqualityWhenEqualShouldBeTrue(decimal left,
                                              decimal right,
                                              bool expectedEquality)
    {
        Money moneyLeft = new(left);
        Money moneyRight = new(right);
        Assert.Equal(expectedEquality, moneyLeft.Equals(moneyRight));
        Assert.Equal(expectedEquality, moneyLeft == moneyRight);
        Assert.Equal(!expectedEquality, moneyLeft != moneyRight);
    }
#pragma warning restore CS8602, CS8625

}
