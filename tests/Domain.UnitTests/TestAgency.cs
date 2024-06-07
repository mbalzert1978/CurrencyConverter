namespace Domain.UnitTests;

public class AgencyTests
{
    [Fact]
    public void GetRatesWhenNoRatesAreAddedShouldReturnEmptyCollection()
    {
        // Arrange
        var agency = Agency.Create("Test Agency", "123 Test St", "Test Country", "USD");

        // Act
        var rates = agency.GetRates();

        // Assert
        Assert.Empty(rates);
    }

    [Fact]
    public void GetRatesWhenRatesAreAddedShouldReturnAllAddedRates()
    {
        // Arrange
        var agency = Agency.Create("Test Agency", "123 Test St", "Test Country", "USD");
        var updateStrategy = new TestUpdateStrategy(
        [
            new("USD", "EUR", "0.85", "2023-10-01T00:00:00"),
                new("USD", "JPY", "110.00", "2023-10-01T00:00:00")
        ]);

        // Act
        agency.Update(updateStrategy);
        var rates = agency.GetRates();

        // Assert
        Assert.Contains(Rate.TryFromStr("USD", "EUR", "0.85", "2023-10-01T00:00:00"), rates);
        Assert.Contains(Rate.TryFromStr("USD", "JPY", "110.00", "2023-10-01T00:00:00"), rates);
    }

    [Fact]
    public void GetRateWhenValidDateStringProvidedButNoMatchingRateShouldReturnNull()
    {
        // Arrange
        var agency = Agency.Create("Test Agency", "123 Test St", "Test Country", "USD");
        agency.AddRate("USD", "EUR", "0.85", "2023-10-01T00:00:00");

        // Act
        var rate = agency.GetRate("USD", "EUR", DateTime.Parse("2023-09-01T00:00:00"));

        // Assert
        Assert.Null(rate);
    }

    public static IEnumerable<object[]> GetRateTestcases =>
    [
        ["GBP", "USD", null, "2"],
        ["USD", "JPY", null, "10"],
        ["USD", "EUR", null, "1"],
        ["EUR", "JPY", null, "10"], // 
        ["USD", "JPY", DateTime.Parse("2023-08-01T00:00:00"), "120.00"],
    ];

    [Theory]
    [MemberData(nameof(GetRateTestcases))]
    public void GetRateWhenNoBaseShouldCalculateRate(string currencyFrom, string currencyTo, DateTime? date, string expectedRate)
    {

        var agency = Agency.Create("Test Agency", "123 Test St", "Test Country", "USD");
        agency.AddRate("USD", "EUR", "1", "2023-10-01T00:00:00"); // 1 / 1 = 1
        agency.AddRate("USD", "GBP", "0.5", "2023-10-01T00:00:00"); // 1 / 0.5 = 2
        agency.AddRate("USD", "JPY", "10.00", "2023-10-01T00:00:00");
        agency.AddRate("USD", "JPY", "120.00", "2023-08-01T00:00:00");

        Rate rate = agency.GetRate(currencyFrom, currencyTo, date);

        Assert.Equal(Money.TryFromStr(expectedRate), rate.Amount);
    }
}

public class TestUpdateStrategy(List<UnprocessedRate> rates) : IUpdateStrategy
{
    private readonly List<UnprocessedRate> _rates = rates;

    public List<UnprocessedRate> Execute()
    {
        return _rates;
    }
}