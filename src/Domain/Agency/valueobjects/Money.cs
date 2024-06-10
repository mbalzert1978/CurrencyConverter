using System.Globalization;

namespace Domain;

public class Money : ValueObject
{
    public static readonly Money Default = new();

    internal decimal Amount { get; private init; }
    internal const string EmptyMessage = "Money amount cannot be empty.";
    internal const string InvalidCharactersMessage = "Money amount can only contain numbers and/or a dot.";
    internal const string ParsingErrorMessage = "Money amount could not be parsed.";
    internal const string NotNegativeMessage = "Money amount cannot be negative.";
    internal const string OverflowExceptionMessage = "Money amount could not be parsed. Overflow occurred.";

    internal Money(decimal amount) => Amount = amount;

    internal Money() => Amount = 0;

    public static Money TryFromStr(string? amount, out Error error, IFormatProvider? invariantCulture = default)
    {
        invariantCulture ??= CultureInfo.InvariantCulture;
        amount = amount?.Replace(',', '.');

        if (string.IsNullOrWhiteSpace(amount))
        {
            error = Error.BadRequest(EmptyMessage);
            return Default;
        }

        if (amount.Any(c => !char.IsAsciiDigit(c) && c != '.'))
        {
            error = Error.BadRequest(InvalidCharactersMessage);
            return Default;
        }

        if (amount.Count(dot => dot == '.') > 1)
        {
            error = Error.BadRequest(ParsingErrorMessage);
            return Default;
        }

        if (!decimal.TryParse(amount, NumberStyles.Any, invariantCulture, out decimal parsed))
        {
            error = Error.BadRequest(OverflowExceptionMessage);
            return Default;
        }

        if (parsed <= 0)
        {
            error = Error.BadRequest(NotNegativeMessage);
            return Default;
        }

        error = Error.None;
        return new Money(Math.Round(parsed, 8));
    }

    internal static void TryFromDecimal(decimal amount, out Money money, out Error error)
    {
        if (amount <= 0)
        {
            money = Default;
            error = Error.BadRequest(NotNegativeMessage);
        }
        else
        {
            money = new Money(Math.Round(amount, 8));
            error = Error.None;
        }
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