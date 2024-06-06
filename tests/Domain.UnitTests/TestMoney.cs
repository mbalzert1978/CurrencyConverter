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
    public void MoneyFromStrWhenValidValueShouldReturnExpectedResults(string amount,
                                                                      decimal expectedAmount) =>
        // Act & Assert
        Assert.True(Money.FromStr(amount).Equals(Money.FromDecimal(expectedAmount)));


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
    public void CreateWhenMoneyIsInvalidShouldRaiseException(string code,
                                                                          string expectedErrorMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<Error>(() => Money.FromStr(code));
        Assert.Equal(expectedErrorMessage, exception.Description);
    }


}