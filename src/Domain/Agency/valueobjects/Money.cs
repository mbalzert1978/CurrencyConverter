using System.Globalization;

namespace Domain;

public class Money : ValueObject
{
    public static readonly Money Default = new();

    internal decimal Amount { get; private init; }
    internal const string EmptyErrorMessage = "Money amount cannot be empty.";
    internal const string InvalidCharactersErrorMessage =
        "Money amount can only contain numbers and/or a dot.";
    internal const string ParsingErrorMessage = "Money amount could not be parsed.";
    internal const string NotNegativeErrorMessage = "Money amount cannot be negative.";

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
        if (string.IsNullOrWhiteSpace(amount))
        {
            money = Default;
            error = new Error("400", EmptyErrorMessage);
            return;
        }

        amount = amount.Replace(",", ".");

        if (amount.Any(c => !char.IsAsciiDigit(c) && c != '.'))
        {
            money = Default;
            error = new Error("400", InvalidCharactersErrorMessage);
            return;
        }

        if (amount.Count(dot => dot == '.') > 1)
        {
            money = Default;
            error = new Error("400", ParsingErrorMessage);
            return;
        }

        if (!decimal.TryParse(amount, invariantCulture, out decimal parsed))
        {
            money = Default;
            error = new Error("400", OverflowExceptionMessage);
            return;
        }

        if (parsed <= 0)
        {
            money = Default;
            error = new Error("400", NotNegativeErrorMessage);
            return;
        }

        money = new Money(Math.Round(parsed, 8));
        error = Error.None;
    }

    internal static void TryFromDecimal(decimal amount, out Money money, out Error error)
    {
        if (amount <= 0)
        {
            money = Default;
            error = new Error("400", NotNegativeErrorMessage);
            return;
        }
        money = new Money(Math.Round(amount, 8));
        error = Error.None;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
    }

    internal Money Multiply(Money amount, out Money money, out Error error)
    {
        TryFromDecimal(Math.Round(Amount * amount.Amount, 8), out money, out error);
        if (error != Error.None)
        {
            throw Error.UnreachableException;
        }
        return money;
    }

    internal Money Invert(out Money money, out Error error)
    {
        TryFromDecimal(Math.Round(1 / Amount, 8), out money, out error);
        if (error != Error.None)
        {
            throw Error.UnreachableException;
        }
        return money;
    }
}
