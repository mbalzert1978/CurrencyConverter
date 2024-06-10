namespace Domain;

public class Agency : AggregateRoot
{
    public static readonly Agency Default =
        new(Guid.Empty, string.Empty, string.Empty, string.Empty, Currency.Default);
    internal string Name { get; set; }
    internal string Address { get; set; }
    internal string Country { get; set; }
    internal Currency BaseCurrency { get; set; }
    internal readonly HashSet<Rate> Rates = [];

    internal Agency(Guid id, string name, string address, string country, Currency baseCurrency)
        : base(id)
    {
        Name = name;
        Address = address;
        Country = country;
        BaseCurrency = baseCurrency;
    }

    public static void TryCreate(
        string name,
        string address,
        string country,
        string code,
        out Agency agency,
        out Error error
    )
    {
        Guid id = Guid.NewGuid();
        Currency.TryFromStr(code, out Currency currency, out Error currencyError);
        if (currencyError != Error.None)
        {
            error = currencyError;
            agency = Default;
            return;
        }

        error = Error.None;
        agency = new(id, name, address, country, currency);
        return;
    }

    public void TryUpdate(IUpdateStrategy updateStrategy, out Error error)
    {
        foreach (UnprocessedRate unprocessed in updateStrategy.Execute())
        {
            AddRate(
                unprocessed.CurrencyFrom,
                unprocessed.CurrencyTo,
                unprocessed.Rate,
                unprocessed.Date,
                out error
        );
            if (error != Error.None)
            {
                return;
            }
        }
        error = Error.None;
    }

    internal void AddRate(
        string currencyFrom,
        string currencyTo,
        string amount,
        string dateTime,
        out Error error
    )
    {
        var created = Rate.TryFromStr(
            currencyFrom,
            currencyTo,
            amount,
            dateTime,
            out error
        );

        if (error != Error.None)
        {
            return;
        }

        Rates.Add(created);
        return;
    }

    public IReadOnlyCollection<Rate> GetRates() => Rates;

    public Rate GetRate(string currencyFrom, string currencyTo, DateTime? dateTime, out Error error)
    {
        bool dateTimeFilter(Rate r) => !dateTime.HasValue || r.DateTime == dateTime;

        Rate? FindRate(Func<Rate, bool> predicate) =>
            GetRates().Where(predicate).OrderByDescending(r => r.DateTime).FirstOrDefault();

        Currency.TryFromStr(currencyFrom, out Currency from, out error);
        if (error != Error.None)
        {
            return Rate.Default;
        }

        Currency.TryFromStr(currencyTo, out Currency to, out error);
        if (error != Error.None)
        {
            return Rate.Default;
        }


        if (from == BaseCurrency)
        {
            Rate? rate = FindRate(r => r.CurrencyFrom == from && r.CurrencyTo == to && dateTimeFilter(r));
            if (rate == null)
            {
                error = Error.NotFound;
                return Rate.Default;
            }

            error = Error.None;
            return rate;
        }

        if (to == BaseCurrency)
        {
            Rate? rate = FindRate(r => r.CurrencyFrom == to && r.CurrencyTo == from && dateTimeFilter(r));
            if (rate == null)
            {
                error = Error.NotFound;
                return Rate.Default;
            }

            var inverted = rate.Invert(out error);
            if (error != Error.None)
            {
                return Rate.Default;
            }

            error = Error.None;
            return inverted;
        }

        var left = FindRate(r => r.CurrencyTo == from && dateTimeFilter(r));
        var right = FindRate(r => r.CurrencyTo == to && dateTimeFilter(r));

        if (left == null || right == null)
        {
            error = Error.NotFound;
            return Rate.Default;
        }

        var multiply = left.Multiply(right, out error);
        if (error != Error.None)
        {
            return Rate.Default;
        }

        var result = multiply.Invert(out error);
        if (error != Error.None)
        {
            return Rate.Default;
        }
        error = Error.None;
        return result;

    }
}

public interface IUpdateStrategy
{
    List<UnprocessedRate> Execute();
}

public record UnprocessedRate(string CurrencyFrom, string CurrencyTo, string Rate, string Date);
