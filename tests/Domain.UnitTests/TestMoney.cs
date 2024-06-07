namespace UnitTests;

using Domain;

public class MoneyTests
{
    [Fact]
    public void TryFromStrWhenAmountIsEmptyShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        string amount = string.Empty;

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.EmptyErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenAmountIsNullShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        string amount = "   ";

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.EmptyErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenAmountContainsInvalidCharactersShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        string amount = "12.34a";

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.InvalidCharactersErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenAmountContainsMultipleDotsShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        string amount = "12.34.56";

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.ParsingErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenAmountIsNegativeShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        string amount = "-12.34";

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.InvalidCharactersErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromStrWhenAmountIsValidShouldReturnMoneyAndNoError()
    {
        // Arrange
        string amount = "12.34";

        // Act
        Money.TryFromStr(amount, out var money, out var error);

        // Assert
        Assert.Equal(12.34m, money.Amount);
        Assert.Equal(Error.None, error);
    }

    [Fact]
    public void TryFromDecimalWhenAmountIsNegativeShouldReturnDefaultMoneyAndError()
    {
        // Arrange
        decimal amount = -12.34m;

        // Act
        Money.TryFromDecimal(amount, out var money, out var error);

        // Assert
        Assert.Equal(Money.Default, money);
        Assert.Equal("400", error.Code);
        Assert.Equal(Money.NotNegativeErrorMessage, error.Description);
    }

    [Fact]
    public void TryFromDecimalWhenAmountIsValidShouldReturnMoneyAndNoError()
    {
        // Arrange
        decimal amount = 12.34m;

        // Act
        Money.TryFromDecimal(amount, out var money, out var error);

        // Assert
        Assert.Equal(12.34m, money.Amount);
        Assert.Equal(Error.None, error);
    }

    [Fact]
    public void MoneyEqualityWhenAmountsAreSameShouldBeEqual()
    {
        // Arrange
        var money1 = new Money(12.34m);
        var money2 = new Money(12.34m);

        // Act & Assert
        Assert.Equal(money1, money2);
    }

    [Fact]
    public void MoneyEqualityWhenAmountsAreDifferentShouldNotBeEqual()
    {
        // Arrange
        var money1 = new Money(12.34m);
        var money2 = new Money(56.78m);

        // Act & Assert
        Assert.NotEqual(money1, money2);
    }
}
