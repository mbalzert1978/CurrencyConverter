namespace Domain.UnitTests;

using Domain;

public class CurrencyTests
{
    public static IEnumerable<object[]> ValidCurrencyCodes =>
    [
        ["USD", "USD", true],
        ["USD", "EUR", false],
    ];

    [Theory]
    [MemberData(nameof(ValidCurrencyCodes))]
    public void Creation_and_equality_when_code_is_valid_should_return_expected_result(
        string code, string other, bool expectedEquality)
    {
        // Act & Assert
        Assert.Equal(expectedEquality, Currency.FromStr(code).Equals(Currency.FromStr(other)));
    }


    public static IEnumerable<object[]> InvalidCurrencyCodes =>
        [
            ["US", "Currency code must be exactly 3 characters long."],
            ["U", "Currency code must be exactly 3 characters long."],
            ["USDE", "Currency code must be exactly 3 characters long."],
            ["123", "Currency code can only contain letters."],
            ["", "Currency code cannot be empty."]
        ];

    [Theory]
    [MemberData(nameof(InvalidCurrencyCodes))]
    public void Create_when_code_is_invalid_length_should_raise_currency_exception(
        string code, string expectedErrorMessage)
    {
        // Act & Assert
        var exception = Assert.Throws<Error>(() => Currency.FromStr(code));
        Assert.Equal(expectedErrorMessage, exception.Description);
    }


}