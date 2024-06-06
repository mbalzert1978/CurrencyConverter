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

    private Money(decimal amount)
    {
        Amount = amount;
    }


    public static Money FromStr(string amount)
    {
        if (string.IsNullOrWhiteSpace(amount))
            throw new Error(
                "400", EmptyErrorMessage);

        amount = amount.Replace(",", ".");

        if (amount.Any(c => !char.IsAsciiDigit(c) && c != '.'))
        {
            throw new Error(
                "400", InvalidCharactersErrorMessage);
        }

        if (amount.Count(dot => dot == '.') > 1)
        {
            throw new Error(
                "400", ParsingErrorMessage);
        }

        decimal parsed;

        try
        {
            parsed = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {

            throw new Error("400", ParsingErrorMessage);
        }

        if (parsed <= 0)
        {
            throw new Error("400", NotNegativeErrorMessage);
        }

        return new Money(Math.Round(parsed, 8));

    }
    internal static Money FromDecimal(decimal amount)
    {
        if (amount <= 0)
        {
            throw new Error("400", NotNegativeErrorMessage);
        }

        return new Money(Math.Round(amount, 8));
    }


    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
    }
}