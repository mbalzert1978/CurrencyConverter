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
        string? amount,
        out Money money,
        out Error error,
        IFormatProvider? invariantCulture = default
    )
    {
        invariantCulture ??= CultureInfo.InvariantCulture;
        decimal parsed = 0;

        amount = amount?.Replace(',', '.');

        (money, error) = amount switch
        {
            null => (Default, Error.BadRequest(EmptyMessage)),
            { } when string.IsNullOrWhiteSpace(amount) => (Default, Error.BadRequest(EmptyMessage)),
            { } when amount.Any(c => !char.IsAsciiDigit(c) && c != '.') => (Default, Error.BadRequest(InvalidCharactersMessage)),
            { } when amount.Count(dot => dot == '.') > 1 => (Default, Error.BadRequest(ParsingErrorMessage)),
            { } when !decimal.TryParse(amount, invariantCulture, out parsed) => (Default, Error.BadRequest(OverflowExceptionMessage)),
            { } when parsed <= 0 => (Default, Error.BadRequest(NotNegativeMessage)),
            _ => (new Money(Math.Round(parsed, 8)), Error.None)
        };


    }

    internal static void TryFromDecimal(decimal amount, out Money money, out Error error)
    {
        (money, error) = amount switch
        {
            { } when amount <= 0 => (Default, Error.BadRequest(NotNegativeMessage)),
            _ => (new Money(Math.Round(amount, 8)), Error.None)
        };
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
    }

    internal Money Multiply(Money amount, out Error error)
    {
        TryFromDecimal(Math.Round(Amount * amount.Amount, 8), out Money money, out error);
        return error == Error.None ? money : Default;

    }

    internal Money Invert(out Error error)
    {
        if (Amount != 0)
        {
            TryFromDecimal(Math.Round(1 / Amount, 8), out Money money, out error);
            return error == Error.None ? money : Default;
        }

        error = Error.Unreachable;
        return Default;
    }
}
