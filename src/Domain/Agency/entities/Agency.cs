namespace Domain;

public class Agency : AggregateRoot
{
    private const string ErrorMessage = "Rate not found.";
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
        Rate.TryFromStr(
            currencyFrom,
            currencyTo,
            amount,
            dateTime,
            out Rate created,
            out Error rateError
        );

        if (rateError != Error.None)
        {
            error = rateError;
            return;
        }

        Rates.Add(created);

        error = Error.None;
        return;
    }

    public IReadOnlyCollection<Rate> GetRates() => Rates;

    public Rate? GetRate(string currencyFrom, string currencyTo, DateTime? dateTime, out Error error)
    {
        bool dateTimeFilter(Rate r) => !dateTime.HasValue || r.DateTime == dateTime;

        Rate? FindRate(Func<Rate, bool> predicate) =>
            GetRates().Where(predicate).OrderByDescending(r => r.DateTime).FirstOrDefault();

        Currency.TryFromStr(currencyFrom, out Currency from, out Error errorFrom);
        if (errorFrom != Error.None)
        {
            error = errorFrom;
            return null;
        }

        Currency.TryFromStr(currencyTo, out Currency to, out errorFrom);
        if (errorFrom != Error.None)
        {
            error = errorFrom;
            return null;
        }


        if (from == BaseCurrency)
        {
            Rate? rate = FindRate(r => r.CurrencyFrom == from && r.CurrencyTo == to && dateTimeFilter(r));
            if (rate == null)
            {
                error = new Error(StatusCode.NotFound, ErrorMessage);
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
                error = new Error(StatusCode.NotFound, ErrorMessage);
                return Rate.Default;
            }

            var inverted = rate.Invert(out errorFrom);
            if (errorFrom != Error.None)
            {
                error = errorFrom;
                return Rate.Default;
            }

            error = Error.None;
            return inverted;
        }

        var left = FindRate(r => r.CurrencyTo == to && dateTimeFilter(r));
        var right = FindRate(r => r.CurrencyTo == from && dateTimeFilter(r));

        if (left == null || right == null)
        {
            error = new Error(StatusCode.NotFound, ErrorMessage);
            return Rate.Default;
        }

        var multiply = left.Multiply(right, out errorFrom);
        if (errorFrom != Error.None)
        {
            error = errorFrom;
            return Rate.Default;
        }

        var result = multiply.Invert(out errorFrom);
        if (errorFrom != Error.None)
        {
            error = errorFrom;
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
