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

    public static Rate TryFromStr(string currencyFrom, string currencyTo, string amount, string dateTime, out Error error)
    {
        if (!DateTime.TryParse(dateTime, out DateTime parsed))
        {
            error = Error.BadRequest(ParsingErrorMessage);
            return Default;
        }
        parsed = parsed.AddSeconds(-parsed.Second).AddMilliseconds(-parsed.Millisecond);

        var from = Currency.TryFromStr(currencyFrom, out error);
        if (error != Error.None) return Default;

        var to = Currency.TryFromStr(currencyTo, out error);
        if (error != Error.None) return Default;

        var money = Money.TryFromStr(amount, out error);
        if (error != Error.None) return Default;

        error = Error.None;
        return new Rate(from, to, money, parsed);
    }

    public Rate Multiply(Rate other, out Error error)
    {
        var money = Amount.Multiply(other.Amount, out error);
        return error == Error.None ? new Rate(other.CurrencyFrom, CurrencyTo, money, DateTime) : Default;
    }

    public Rate Invert(out Error error)
    {
        var money = Amount.Invert(out error);
        return error == Error.None ? new Rate(CurrencyTo, CurrencyFrom, money, DateTime) : Default;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return CurrencyFrom;
        yield return CurrencyTo;
        yield return Amount;
        yield return DateTime;
    }
}