namespace Domain;

public class Money : ValueObject
{
    protected decimal Amount { get; private init; }
    private const string EmptyErrorMessage =
        "Money amount cannot be empty.";
    private const string InvalidCharactersErrorMessage =
        "Money amount can only contain numbers and/or a dot.";
    private const string ParsingErrorMessage =
        "Money amount could not be parsed.";
    private const string NotNegativeErrorMessage =
        "Money amount cannot be negative.";

    private const string OverflowExceptionMessage =
        "Money amount could not be parsed. Overflow occurred.";

    private Money(decimal amount)
    {
        Amount = amount;
    }


    public static bool TryFromStr(string amount, out Money? money, out Error? error)
    {
        if (string.IsNullOrWhiteSpace(amount))
        {
            money = null;
            error = new Error(
            "400", EmptyErrorMessage);
            return false;
        }

        amount = amount.Replace(",", ".");

        if (amount.Any(c => !char.IsAsciiDigit(c) && c != '.'))
        {
            money = null;
            error = new Error(
                "400", InvalidCharactersErrorMessage);
        }

        if (amount.Count(dot => dot == '.') > 1)
        {
            error = new Error(
                "400", ParsingErrorMessage);
        }

        if (!decimal.TryParse(amount, System.Globalization.CultureInfo.InvariantCulture, out decimal parsed))
        {
            money = null;
            error = new Error(
                "400", OverflowExceptionMessage);
            return false;
        }

        if (parsed <= 0)
        {
            money = null;
            error = new Error(
                "400", NotNegativeErrorMessage);
            return false;
        }

        money = new Money(Math.Round(parsed, 8));
        error = null;
        return true;

    }
    internal static bool TryFromDecimal(decimal amount, out Money? money, out Error? error)
    {
        if (amount <= 0)
        {
            money = null;
            error = new Error("400", NotNegativeErrorMessage);
            return false;
        }
        money = new Money(Math.Round(amount, 8));
        error = null;
        return true;
    }


    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
    }

    internal bool Multiply(Money amount, out Money? money, out Error? error)
    {
        return TryFromDecimal(Math.Round(Amount * amount.Amount, 8), out money, out error);
    }

    internal bool Invert(out Money? money, out Error? error)
    {
        return TryFromDecimal(Math.Round(1 / Amount, 8), out money, out error);
    }
}