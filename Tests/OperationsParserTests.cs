using Acm.DB;
using Acm.OperationsParser;

namespace Acm.Tests;

public class Tests
{
    private readonly OperationsParser.OperationsParser parser_ = new(new OperationsParserOptions());

    private static string GetDataPath(string path)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, path);
    }

    [Test]
    public void Test01Success()
    {
        var result = parser_.Parse(GetDataPath("TestData/Parse1.xlsx"));
        Assert.That(result, Is.EqualTo(new List<Transaction>
        {
            new()
            {
                SettlementDate = new DateTime(2024, 07, 08),
                Id = "0012",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Noname Inc",
                Ticker = "NNM US",
                ISIN = "ISIN1",
                Quantity = 20,
                Price = 7797.98m / 20,
                Total = -7797.98m,
                Currency = "USD",
                SourceIndex = 5
            },
            new()
            {
                SettlementDate = new DateTime(2024, 08, 29),
                Id = "0356/123",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Nobody Inc",
                Ticker = "NBDY US",
                ISIN = "ISIN2",
                Quantity = -495,
                Price = 6841.52m / 495,
                Total = 6841.52m,
                Currency = "USD",
                SourceIndex = 6
            },
            new()
            {
                SettlementDate = new DateTime(2024, 09, 03),
                Id = "2154/123",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Big Brother",
                Ticker = "BB US",
                ISIN = "ISIN3",
                Quantity = -68,
                Price = 4749.73m / 68,
                Total = 4749.73m,
                Currency = "USD",
                SourceIndex = 8
            },
            new()
            {
                SettlementDate = new DateTime(2024, 09, 03),
                Id = "7784/123",
                Type = BuySell.Buy,
                AssetType = "ETF",
                Asset = "Progress Inc",
                Ticker = "PRG US",
                ISIN = "ISIN4",
                Quantity = 435,
                Price = 6137.63m / 435,
                Total = -6137.63m,
                Currency = "USD",
                SourceIndex = 10
            }
        }));
    }

    [Test]
    public void Test02Success()
    {
        var result = parser_.Parse(GetDataPath("TestData/Parse2.xlsx"));
        Assert.That(result, Is.EqualTo(new List<Transaction>
        {
            new()
            {
                SettlementDate = new DateTime(2024, 10, 08),
                Id = "00358017/113",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "12345 Inc",
                Ticker = "12345 US",
                ISIN = "US12345",
                Quantity = 12,
                Price = 1371.83m / 12,
                Total = -1371.83m,
                Currency = "USD",
                SourceIndex = 4
            },
            new()
            {
                SettlementDate = new DateTime(2024, 10, 08),
                Id = "00358021/113",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "XXXX Corp",
                Ticker = "XXXX US",
                ISIN = "USXXXX",
                Quantity = 21,
                Price = 2661.32m / 21,
                Total = -2661.32m,
                Currency = "USD",
                SourceIndex = 5
            },
            new()
            {
                SettlementDate = new DateTime(2024, 10, 09),
                Id = "00358174/113",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Mean Corp",
                Ticker = "MEAN US",
                ISIN = "USMEAN",
                Quantity = -23,
                Price = 9501.54m / 23,
                Total = 9501.54m,
                Currency = "USD",
                SourceIndex = 6
            },
            new()
            {
                SettlementDate = new DateTime(2024, 10, 15),
                Id = "00358652/113",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Loopback Inc",
                Ticker = "LOOP US",
                ISIN = "USLOOP",
                Quantity = 17,
                Price = 1205.64m / 17,
                Total = -1205.64m,
                Currency = "USD",
                SourceIndex = 7
            },
            new()
            {
                SettlementDate = new DateTime(2024, 10, 15),
                Id = "00358974/113",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Braindead Corp",
                Ticker = "BRD US",
                ISIN = "USDEAD",
                Quantity = 48,
                Price = 9172.43m / 48,
                Total = -9172.43m,
                Currency = "USD",
                SourceIndex = 8
            },
            new()
            {
                SettlementDate = new DateTime(2024, 10, 24),
                Id = "00360775/113",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "DRUNK Inc",
                Ticker = "DRNK US",
                ISIN = "USDRUNK",
                Quantity = -61,
                Price = 1795.60m / 61,
                Total = 1795.60m,
                Currency = "USD",
                SourceIndex = 10
            }
        }));
    }

    [Test]
    public void Test03MissingColumnFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath("TestData/NoColumn.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Failed to locate some columns in the input. Found 12 expected 13"));
    }

    [Test]
    public void Test04BadUnitsFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath("TestData/BadUnits.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Unexpected measurement units parrots"));
    }

    [Test]
    public void Test05BadOperationTypeFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath("TestData/BadOperationType.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Unexpected operation type ЛИКВИДАЦИЯ"));
    }

    [Test]
    public void Test06PriceMismatchFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath("TestData/PriceMismatch.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo($"Calculated transaction price {389.899m} does not match the data price {389.91m}"));
    }
}
