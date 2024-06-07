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


    public static bool TryFromStr(string currencyFrom, string currencyTo, string amount, string dateTime, out Rate? rate, out Error? error)
    {
        if (!DateTime.TryParse(dateTime, out DateTime parsed))
        {
            rate = null;
            error = new Error("400", ParsingErrorMessage);
            return false;
        }
        parsed = parsed.AddSeconds(-parsed.Second)
               .AddMilliseconds(-parsed.Millisecond);
        if (!Currency.TryFromStr(currencyFrom, out Currency? from, out Error? fromError))
        {
            rate = null;
            error = fromError;
            return false;
        }
        if (!Currency.TryFromStr(currencyTo, out Currency? to, out Error? toError))
        {
            rate = null;
            error = toError;
            return false;
        }
        if (!Money.TryFromStr(amount, out Money? money, out Error? moneyError))
        {
            rate = null;
            error = moneyError;
            return false;
        }
        rate = new Rate(from, to, money, parsed);
        error = null;
        return true;

    }

    public bool Multiply(Rate other, out Rate? rate, out Error? error)
    {
        if (!Amount.Multiply(other.Amount, out Money? money, out Error? moneyError))
        {
            rate = null;
            error = moneyError;
            return false;
        }
        rate = new Rate(other.CurrencyFrom, CurrencyTo, money, DateTime);
        error = null;
        return true;
    }

    public bool Invert(out Rate? rate, out Error? error)
    {
        if (!Amount.Invert(out Money? money, out Error? moneyError))
        {
            rate = null;
            error = moneyError;
            return false;
        }
        rate = new Rate(CurrencyTo, CurrencyFrom, money, DateTime);
        error = null;
        return true;
    }



    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyFrom;
        yield return CurrencyTo;
        yield return Amount;
        yield return DateTime;
    }
}