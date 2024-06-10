namespace UnitTests;

using Domain;

public class AgencyTests
{
#pragma warning disable CS8604, CS8625
    public static IEnumerable<object[]> AgencyCreateTestCases =>
    [
        ["USD", "foo", "bar", "baz", false, string.Empty,string.Empty],
        ["invalid", "foo", "bar", "baz", true, StatusCode.BadRequest, Currency.InvalidLengthMessage],
    ];
    [Theory]
    [MemberData(nameof(AgencyCreateTestCases))]
    public void TryCreateWhenCalledShouldReturnExpectedResult(
        string code,
        string name,
        string address,
        string country,
        bool isError,
        string statusCode,
        string expectedMessage
    )
    {
        Agency.TryCreate(name, address, country, code, out var agency, out Error error);

        if (isError)
        {
            Assert.Equal(statusCode, error.Code);
            Assert.Equal(expectedMessage, error.Description);
        }
        else
        {
            Assert.Equal(name, agency.Name);
            Assert.Equal(address, agency.Address);
            Assert.Equal(country, agency.Country);
            Assert.Equal(code, agency.BaseCurrency.Code);
            Assert.Equal(Error.None, error);
        }
    }

    public static IEnumerable<object[]> AddRateTestCases =>
    [
        ["USD", "EUR", "1,23", "2023-10-01T00:00:00", false, string.Empty,string.Empty],
        ["USD", "EUR", "1,23", "invalid-date", true, StatusCode.BadRequest, Rate.ParsingErrorMessage],
        ["invalid", "EUR", "1,23", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Currency.InvalidLengthMessage],
        ["USD", "invalid", "1,23", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Currency.InvalidLengthMessage],
        ["USD", "EUR", "invalid", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Money.InvalidCharactersMessage],
        ["USD", "EUR", "invalid", "2023-10-01T00:00:00", true, StatusCode.BadRequest, Money.InvalidCharactersMessage],
    ];
    [Theory]
    [MemberData(nameof(AddRateTestCases))]
    public void AddRateWhenCalledShouldReturnExpectedResults(
        string currencyFrom,
        string currencyTo,
        string amount,
        string dateTime,
        bool isError,
        string statusCode,
        string expectedMessage
    )
    {
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );

        agency.AddRate(currencyFrom, currencyTo, amount, dateTime, out var error);

        if (isError)
        {
            Assert.Equal(statusCode, error.Code);
            Assert.Equal(expectedMessage, error.Description);
        }
        else
        {
            Assert.Equal(Error.None, error);
            Assert.Single(agency.GetRates());
            Assert.Equal(new Rate(new Currency(currencyFrom),
                                  new Currency(currencyTo),
                                  new Money(decimal.Parse(amount)),
                                  DateTime.Parse(dateTime)), agency.GetRates().First());
        }
    }

    public static IEnumerable<object[]> GetRateTestCases =>
    [
        ["USD", "EUR", DateTime.Parse("2023-10-01T00:00:00"), 1.00, false, string.Empty, string.Empty],
        ["USD", "EUR", DateTime.Parse("2023-10-02T00:00:00"), 1.25, false, string.Empty, string.Empty],
        ["EUR", "USD", DateTime.Parse("2023-10-02T00:00:00"), 0.8, false, string.Empty, string.Empty],
        ["EUR", "JPY", DateTime.Parse("2023-10-01T00:00:00"), 100, false, string.Empty, string.Empty],
        ["EUR", "JPY", DateTime.Parse("2023-10-02T00:00:00"), 64, false, string.Empty, string.Empty],
        ["EUR", "JPY", null, 44.44444444, false, string.Empty, string.Empty],
        ["USD", "GBP", DateTime.Parse("2023-10-01T00:00:00"), 1.00, true, StatusCode.NotFound, Error.NotFoundMessage],
        ["USD", "GBP", null, 1.00, true, StatusCode.NotFound, Error.NotFoundMessage],

    ];

    [Theory]
    [MemberData(nameof(GetRateTestCases))]
    public void GetRateWhenCalledShouldReturnExpected(
        string currencyFrom,
        string currencyTo,
        DateTime? dateTime,
        decimal expectedAmount,
        bool isError,
        string statusCode,
        string expectedMessage
    )
    {
        var agency = new Agency(
            Guid.NewGuid(),
            "Test Agency",
            "123 Test St",
            "Testland",
            new Currency("USD")
        );
        agency.AddRate(agency.BaseCurrency.Code, "EUR", "1", "2023-10-01T00:00:00", out _);
        agency.AddRate(agency.BaseCurrency.Code, "EUR", "1.25", "2023-10-02T00:00:00", out _);
        agency.AddRate(agency.BaseCurrency.Code, "EUR", "1.5", "2023-10-03T00:00:00", out _);
        agency.AddRate(agency.BaseCurrency.Code, "JPY", "0.01", "2023-10-01T00:00:00", out _);
        agency.AddRate(agency.BaseCurrency.Code, "JPY", "0.0125", "2023-10-02T00:00:00", out _);
        agency.AddRate(agency.BaseCurrency.Code, "JPY", "0.0150", "2023-10-03T00:00:00", out _);

        var rate = agency.GetRate(currencyFrom, currencyTo, dateTime, out Error error);

        if (isError)
        {
            Assert.Equal(statusCode, error.Code);
            Assert.Equal(expectedMessage, error.Description);
        }
        else
        {
            Assert.Equal(new Money(expectedAmount), rate.Amount);
            Assert.Equal(Error.None, error);
        }
    }

#pragma warning restore CS8604, CS8625
}
