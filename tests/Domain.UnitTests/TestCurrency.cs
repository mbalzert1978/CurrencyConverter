namespace Domain.UnitTests;

using Domain;

public class CurrencyTests
{
    // public static IEnumerable<object[]> ValidCurrencyCodes =>
    // [
    //     ["USD", "USD", true],
    //     ["USD", "EUR", false],
    // ];

    // [Theory]
    // [MemberData(nameof(ValidCurrencyCodes))]
    // public void Creation_and_equality_when_code_is_valid_should_return_expected_result(
    //     string code, string other, bool expectedEquality)
    // {
    //     // Act & Assert
    //     Assert.Equal(expectedEquality, Currency.TryFromStr(code).Equals(Currency.TryFromStr(other)));
    // }


    // public static IEnumerable<object[]> InvalidCurrencyCodes =>
    //     [
    //         ["US", "Currency code must be exactly 3 characters long."],
    //         ["U", "Currency code must be exactly 3 characters long."],
    //         ["USDE", "Currency code must be exactly 3 characters long."],
    //         ["123", "Currency code can only contain letters."],
    //         ["", "Currency code cannot be empty."]
    //     ];

    // [Theory]
    // [MemberData(nameof(InvalidCurrencyCodes))]
    // public void Create_when_code_is_invalid_length_should_raise_currency_exception(
    //     string code, string expectedErrorMessage)
    // {
    //     // Act & Assert
    //     var exception = Assert.Throws<Error>(() => Currency.TryFromStr(code));
    //     Assert.Equal(expectedErrorMessage, exception.Description);
    // }

    public static IEnumerable<object[]> CurrencyTestCases =>
    [
        ["USD", new Currency("USD"), null],
        ["USD", new Currency("EUR"), null],
        ["US", null, "Currency code must be exactly 3 characters long."],
        ["U", null, "Currency code must be exactly 3 characters long."],
        ["USDE", null, "Currency code must be exactly 3 characters long."],
        ["123", null, "Currency code can only contain letters."],
        ["", null, "Currency code cannot be empty."]
    ];

    [Theory]
    [MemberData(nameof(CurrencyTestCases))]
    public void Currency_Tests(string code, Currency? expectedCurrency, string? expectedErrorMessage)
    {
        // Act
        var result = Currency.TryFromStr(code, out var currency, out var error);

        // Assert
        if (result)
        {
            Assert.Null(error);
            Assert.Equal(expectedCurrency, currency);
        }
        else
        {
            Assert.Null(currency);
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Description);
        }

    }


}