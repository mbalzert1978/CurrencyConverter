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
                out Error rateError
            );
            if (rateError != Error.None)
            {
                error = rateError;
                return;
            }
        }
        error = Error.None;
        return;
    }

    internal void AddRate(
        string currencyFrom,
        string currencyTo,
        string amount,
        string dateTime,
        out Error error
    )
    {
        // Attempt to create a Rate object from the provided parameters.
        Rate.TryFromStr(
            currencyFrom,
            currencyTo,
            amount,
            dateTime,
            out Rate created,
            out Error rateError
        );

        // If there was an error creating the Rate object, set the error output parameter and return.
        if (rateError != Error.None)
        {
            error = rateError;
            return;
        }

        // Add the created Rate object to the collection of rates.
        Rates.Add(created);

        // Set the error output parameter to indicate success.
        error = Error.None;
        return;
    }

    public IReadOnlyCollection<Rate> GetRates() => Rates;

    public Rate? GetRate(string currencyFrom, string currencyTo, DateTime? dateTime)
    {
        Currency.TryFromStr(currencyFrom, out Currency from, out Error fromError);
        if (fromError != Error.None)
        {
            return null;
        }
        Currency.TryFromStr(currencyTo, out Currency to, out Error toError);
        if (toError != Error.None)
        {
            return null;
        }

        bool dateTimeFilter(Rate r) => !dateTime.HasValue || r.DateTime == dateTime;

        Rate? FindRate(Func<Rate, bool> predicate) =>
            GetRates().Where(predicate).OrderByDescending(r => r.DateTime).FirstOrDefault();

        if (from == BaseCurrency)
        {
            return FindRate(r => r.CurrencyFrom == from && r.CurrencyTo == to && dateTimeFilter(r));
        }

        if (to == BaseCurrency)
        {
            return FindRate(r => r.CurrencyFrom == to && r.CurrencyTo == from && dateTimeFilter(r))
                ?.Invert();
        }

        Rate? left = FindRate(r => r.CurrencyTo == to && dateTimeFilter(r));
        Rate? right = FindRate(r => r.CurrencyTo == from && dateTimeFilter(r));

        return left != null && right != null ? left.Multiply(right.Invert()) : null;
    }
}

public interface IUpdateStrategy
{
    List<UnprocessedRate> Execute();
}

public record UnprocessedRate(string CurrencyFrom, string CurrencyTo, string Rate, string Date);
