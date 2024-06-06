namespace Domain;

public class Rate : ValueObject
{
    public Currency CurrencyFrom { get; private init; }
    public Currency CurrencyTo { get; private init; }
    public Money Amount { get; private init; }
    public DateTime DateTime { get; private init; }
    private const string ParsingErrorMessage =
        "DateTime is not in a valid ISO 8601 format.";

    private Rate(Currency currencyFrom, Currency currencyTo, Money amount, DateTime dateTime)
    {
        CurrencyFrom = currencyFrom;
        CurrencyTo = currencyTo;
        Amount = amount;
        DateTime = dateTime;
    }


    public static Rate FromStr(string currencyFrom, string currencyTo, string amount, string dateTime)
    {
        DateTime parsed;
        try
        {
            parsed = DateTime.Parse(dateTime)
                .AddSeconds(-DateTime.Parse(dateTime).Second)
                .AddMilliseconds(-DateTime.Parse(dateTime).Millisecond);
        }
        catch (Exception)
        {

            throw new Error("400", ParsingErrorMessage);
        }

        return new(
            Currency.FromStr(currencyFrom),
            Currency.FromStr(currencyTo),
            Money.FromStr(amount),
            parsed
        );

    }

    public Rate Multiply(Rate other)
    {
        return new(
            other.CurrencyFrom,
            CurrencyTo,
            Amount.Multiply(other.Amount),
            DateTime
        );
    }

    public Rate Invert()
    {
        return new(
            CurrencyTo,
            CurrencyFrom,
            Amount.Invert(),
            DateTime
        );
    }



    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyFrom;
        yield return CurrencyTo;
        yield return Amount;
        yield return DateTime;
    }
}