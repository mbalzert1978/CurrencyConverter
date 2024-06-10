namespace Domain;

public class Rate : ValueObject
{
    public Currency CurrencyFrom { get; private init; }
    public Currency CurrencyTo { get; private init; }
    public Money Amount { get; private init; }
    public DateTime DateTime { get; private init; }
    public static readonly Rate Default = new();
    internal const string ParsingErrorMessage = "DateTime is not in a valid ISO 8601 format.";

    internal Rate(Currency currencyFrom, Currency currencyTo, Money amount, DateTime dateTime)
    {
        CurrencyFrom = currencyFrom;
        CurrencyTo = currencyTo;
        Amount = amount;
        DateTime = dateTime;
    }

    internal Rate()
    {
        CurrencyFrom = Currency.Default;
        CurrencyTo = Currency.Default;
        Amount = Money.Default;
        DateTime = DateTime.MinValue;
    }

    public static void TryFromStr(
        string currencyFrom,
        string currencyTo,
        string amount,
        string dateTime,
        out Rate rate,
        out Error error
    )
    {
        if (!DateTime.TryParse(dateTime, out DateTime parsed))
        {
            rate = Default;
            error = new Error(StatusCode.BadRequest, ParsingErrorMessage);
            return;
        }
        parsed = parsed.AddSeconds(-parsed.Second).AddMilliseconds(-parsed.Millisecond);
        Currency.TryFromStr(currencyFrom, out Currency? from, out Error? fromError);
        if (fromError != Error.None)
        {
            rate = Default;
            error = fromError;
            return;
        }
        Currency.TryFromStr(currencyTo, out Currency? to, out Error? toError);
        if (toError != Error.None)
        {
            rate = Default;
            error = toError;
            return;
        }
        Money.TryFromStr(amount, out Money? money, out Error? moneyError);
        if (moneyError != Error.None)
        {
            rate = Default;
            error = moneyError;
            return;
        }
        rate = new Rate(from, to, money, parsed);
        error = Error.None;
        return;
    }

    public Rate Multiply(Rate other, out Error error)
    {
        Amount.Multiply(other.Amount, out Money money, out Error moneyError);
        if (moneyError != Error.None)
        {
            error = moneyError;
            return Default;
        }
        error = Error.None;
        return new(other.CurrencyFrom, CurrencyTo, money, DateTime);
    }

    public Rate Invert(out Error error)
    {
        Amount.Invert(out Money money, out Error moneyError);
        if (moneyError != Error.None)
        {
            error = moneyError;
            return Default;
        }
        error = Error.None;
        return new(CurrencyTo, CurrencyFrom, money, DateTime);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyFrom;
        yield return CurrencyTo;
        yield return Amount;
        yield return DateTime;
    }
}
