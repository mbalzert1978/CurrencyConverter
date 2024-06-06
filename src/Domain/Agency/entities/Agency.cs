namespace Domain;
public class Agency : AggregateRoot
{
    protected string Name { get; set; }
    protected string Address { get; set; }
    protected string Country { get; set; }
    protected Currency BaseCurrency { get; set; }
    protected HashSet<Rate> Rates { get; set; } = [];

    private Agency(Guid id, string name, string address, string country, Currency baseCurrency) : base(id)
    {
        Name = name;
        Address = address;
        Country = country;
        BaseCurrency = baseCurrency;
    }

    public static Agency Create(string name, string address, string country, string code)
    {
        Guid id = Guid.NewGuid();
        return new(id, name, address, country, Currency.FromStr(code));
    }

    internal void AddRate(string currencyFrom, string currencyTo, string amount, string dateTime)
    {
        Rate rate = Rate.FromStr(currencyFrom, currencyTo, amount, dateTime);
        Rates.Add(rate);
    }

    public void Update(IUpdateStrategy updateStrategy)
    {
        foreach (UnprocessedRate rate in updateStrategy.Execute())
        {
            AddRate(rate.CurrencyFrom, rate.CurrencyTo, rate.Rate, rate.Date);
        }
    }

    public IReadOnlyCollection<Rate> GetRates() => Rates;
    public Rate? GetRate(string currencyFrom, string currencyTo, DateTime? dateTime)
    {
        Currency from;
        Currency to;

        try
        {
            from = Currency.FromStr(currencyFrom);
            to = Currency.FromStr(currencyTo);
        }
        catch (Exception)
        {

            return null;
        }

        bool dateTimeFilter(Rate r) => !dateTime.HasValue || r.DateTime == dateTime;

        Rate? FindRate(Func<Rate, bool> predicate) =>
            GetRates()
                .Where(predicate)
                .OrderByDescending(r => r.DateTime)
                .FirstOrDefault();


        if (from == BaseCurrency)
        {
            return FindRate(r => r.CurrencyFrom == from && r.CurrencyTo == to && dateTimeFilter(r));
        }

        if (to == BaseCurrency)
        {
            return FindRate(r => r.CurrencyFrom == to && r.CurrencyTo == from && dateTimeFilter(r))?.Invert();
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
