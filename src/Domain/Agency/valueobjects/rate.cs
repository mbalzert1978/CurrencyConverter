namespace Domain;

public class Rate : ValueObject
{
    protected Currency CurrencyFrom { get; private init; }
    protected Currency CurrencyTo { get; private init; }
    protected Money Amount { get; private init; }
    protected DateTime DateTime { get; private init; }
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



    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyFrom;
        yield return CurrencyTo;
        yield return Amount;
        yield return DateTime;
    }
}