namespace Domain.UnitTests;

using Domain;

public class MoneyTests
{
    public static IEnumerable<object[]> ValidMoneyAmounts =>
    [
        ["100", decimal.Parse("100.00000000", System.Globalization.CultureInfo.InvariantCulture)],
        ["200", decimal.Parse("200.00000000", System.Globalization.CultureInfo.InvariantCulture)],
        ["1", decimal.Parse("1.00000000", System.Globalization.CultureInfo.InvariantCulture)],
        ["1,25", decimal.Parse("1.25000000", System.Globalization.CultureInfo.InvariantCulture)],
        ["0.123456789", Math.Round(decimal.Parse("0.123456789", System.Globalization.CultureInfo.InvariantCulture),8)],
        ["0.123456780", Math.Round(decimal.Parse("0.123456780", System.Globalization.CultureInfo.InvariantCulture),8)],
    ];

    [Theory]
    [MemberData(nameof(ValidMoneyAmounts))]
    public void MoneyFromStrWhenValidShouldReturnExpectedResults(string amount,
                                                                      decimal expectedAmount) =>
        // Act & Assert
        Assert.True(Money.TryFromStr(amount).Equals(Money.FromDecimal(expectedAmount)));


    public static IEnumerable<object[]> InvalidMoneyAmounts =>
        [
            ["-100", "Money amount can only contain numbers and/or a dot."],
            ["0", "Money amount cannot be negative."],
            ["invalid", "Money amount can only contain numbers and/or a dot."],
            ["100.abc", "Money amount can only contain numbers and/or a dot."],
            ["1,2,2", "Money amount could not be parsed."],
            ["", "Money amount cannot be empty."],
        ];

    [Theory]
    [MemberData(nameof(InvalidMoneyAmounts))]
    public void MoneyFromStrWhenInvalidShouldRaiseException(string code,
                                                                          string expectedErrorMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<Error>(() => Money.TryFromStr(code));
        Assert.Equal(expectedErrorMessage, exception.Description);
    }

    public static IEnumerable<object[]> LargeAmountData =>
        [
            [decimal.MaxValue / 2, 1, decimal.MaxValue / 2],
            [decimal.MaxValue / 2, 0.5m, decimal.MaxValue / 4],
            [decimal.MaxValue / 2, 0.00000001m, decimal.MaxValue / 2 * 0.00000001m]
        ];

    [Theory]
    [MemberData(nameof(LargeAmountData))]
    public void MoneyMultiplyWhenLargeAmountsShouldReturnExpectedResults(decimal left,
                                                                         decimal right,
                                                                         decimal expected) =>
        // Arrange & Act
        Assert.Equal(Money.FromDecimal(expected), Money.FromDecimal(left)
                                                       .Multiply(Money.FromDecimal(right)));

    public static IEnumerable<object[]> LargeAmountErrorData =>
    [
        [decimal.MaxValue / 2, -2, "Money amount cannot be negative."],
        [decimal.MaxValue / 2, 0, "Money amount cannot be negative."],
        [decimal.MaxValue / 2, 0.0000000001m, "Money amount cannot be negative."]
    ];
    [Theory]
    [MemberData(nameof(LargeAmountErrorData))]
    public void MoneyMultiplyWhenInvalidAmountsShouldRaiseException(decimal left,
                                                                decimal right,
                                                                string expectedErrorMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<Error>(() => Money.FromDecimal(left)
                                                        .Multiply(Money.FromDecimal(right)));
        Assert.Equal(expectedErrorMessage, exception.Description);
    }

}