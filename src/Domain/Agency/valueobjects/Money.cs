using System.Globalization;

namespace Domain;

public class Money : ValueObject
{
    public static readonly Money Default = new();

    internal decimal Amount { get; private init; }
    internal const string EmptyMessage =
        "Money amount cannot be empty.";
    internal const string InvalidCharactersMessage =
        "Money amount can only contain numbers and/or a dot.";
    internal const string ParsingErrorMessage =
        "Money amount could not be parsed.";
    internal const string NotNegativeMessage =
        "Money amount cannot be negative.";
    internal const string OverflowExceptionMessage =
        "Money amount could not be parsed. Overflow occurred.";

    internal Money(decimal amount) => Amount = amount;

    internal Money() => Amount = 0;

    public static void TryFromStr(
        string amount,
        out Money money,
        out Error error,
        IFormatProvider? invariantCulture = default
    )
    {
        invariantCulture ??= CultureInfo.InvariantCulture;
        decimal parsed = 0;

        (money, error) = amount switch
        {
            null => (Default, new Error(StatusCode.BadRequest, EmptyMessage)),
            { } when string.IsNullOrWhiteSpace(amount) => (Default, new Error(StatusCode.BadRequest, EmptyMessage)),
            { } when amount.Any(c => !char.IsAsciiDigit(c) && c != '.') => (Default, new Error(StatusCode.BadRequest, InvalidCharactersMessage)),
            { } when amount.Count(dot => dot == '.') > 1 => (Default, new Error(StatusCode.BadRequest, ParsingErrorMessage)),
            { } when !decimal.TryParse(amount, invariantCulture, out parsed) => (Default, new Error(StatusCode.BadRequest, OverflowExceptionMessage)),
            { } when parsed <= 0 => (Default, new Error(StatusCode.BadRequest, NotNegativeMessage)),
            _ => (new Money(Math.Round(parsed, 8)), Error.None)
        };


    }

    internal static void TryFromDecimal(decimal amount, out Money money, out Error error)
    {
        (money, error) = amount switch
        {
            { } when amount <= 0 => (Default, new Error(StatusCode.BadRequest, NotNegativeMessage)),
            _ => (new Money(Math.Round(amount, 8)), Error.None)
        };
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
    }

    internal Money? Multiply(Money amount, out Money money, out Error error)
    {
        TryFromDecimal(Math.Round(Amount * amount.Amount, 8), out money, out error);
        if (error != Error.None)
        {
            money = Default;
            return null;
        }
        return money;
    }

    internal Money? Invert(out Money money, out Error error)
    {
        TryFromDecimal(Math.Round(1 / Amount, 8), out money, out error);
        if (error != Error.None)
        {
            return money;
        }
        return money;
    }
}
