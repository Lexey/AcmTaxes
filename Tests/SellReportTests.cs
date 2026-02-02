using Acm.DB;
using Acm.OperationsParser;
using Acm.SellReport;
using NUnit.Compatibility;

namespace Acm.Tests
{
    public class SellReportTests
    {
        private string tempFilePath_;

        private static readonly SellReportOptions ReportOptions = new SellReportOptions();
        private readonly SellReport.SellReport report_ = new(ReportOptions, new OperationsParserOptions());

        private static readonly List<Transaction> Transactions1 =
        [
            new () {
                SettlementDate = new DateTime(2024, 07, 05),
                Id = "001",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = 253,
                Price = Math.Round(3472.19m / 253, ReportOptions.PriceLoadRoundingDigits),
                Total = -3472.19m,
                Currency = "USD",
                SourceIndex = 1
            },
            new () {
                SettlementDate = new DateTime(2024, 07, 09),
                Id = "002",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = 277,
                Price = Math.Round(3789.00m / 277, ReportOptions.PriceLoadRoundingDigits),
                Total = -3789.00m,
                Currency = "USD",
                SourceIndex = 2
            },
            new () {
                SettlementDate = new DateTime(2024, 09, 19),
                Id = "003/567",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = 105,
                Price = Math.Round(1435.99m / 105, ReportOptions.PriceLoadRoundingDigits),
                Total = -1435.99m,
                Currency = "USD",
                SourceIndex = 3
            },
            new () {
                SettlementDate = new DateTime(2024, 09,26),
                Id = "004/567",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = -157,
                Price = Math.Round(2534.54m / 157, ReportOptions.PriceLoadRoundingDigits),
                Total = 2534.54m,
                Currency = "USD",
                SourceIndex = 4
            },
            new () {
                SettlementDate = new DateTime(2024, 12, 09),
                Id = "005/567",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = 117,
                Price = Math.Round(2028.20m / 117, ReportOptions.PriceLoadRoundingDigits),
                Total = -2028.20m,
                Currency = "USD",
                SourceIndex = 5
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 07),
                Id = "006/567",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = -28,
                Price = Math.Round(219.55m / 28, ReportOptions.PriceLoadRoundingDigits),
                Total = 219.55m,
                Currency = "USD",
                SourceIndex = 6
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 15),
                Id = "007/567",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "Asset1",
                Ticker = "ASS 1",
                ISIN = "CAASS1",
                Quantity = -567,
                Price = Math.Round(5172.24m / 567, ReportOptions.PriceLoadRoundingDigits),
                Total = 5172.24m,
                Currency = "USD",
                SourceIndex = 7
            }
        ];

        private static readonly List<Transaction> Transactions2 =
        [
            new () {
                SettlementDate = new DateTime(2024, 07, 05),
                Id = "001",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 31,
                Price = Math.Round(5204.36m / 31, ReportOptions.PriceLoadRoundingDigits),
                Total = -5204.36m,
                Currency = "USD",
                SourceIndex = 1
            },
            new () {
                SettlementDate = new DateTime(2024, 07, 05),
                Id = "002",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 25,
                Price = Math.Round(3552.09m / 25, ReportOptions.PriceLoadRoundingDigits),
                Total = -3552.09m,
                Currency = "USD",
                SourceIndex = 2
            },
            new () {
                SettlementDate = new DateTime(2024, 07, 09),
                Id = "003",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 30,
                Price = Math.Round(5310.08m / 30, ReportOptions.PriceLoadRoundingDigits),
                Total = -5310.08m,
                Currency = "USD",
                SourceIndex = 3
            },
            new () {
                SettlementDate = new DateTime(2024, 07, 09),
                Id = "004",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 24,
                Price = Math.Round(3507.02m / 24, ReportOptions.PriceLoadRoundingDigits),
                Total = -3507.02m,
                Currency = "USD",
                SourceIndex = 4
            },
            new () {
                SettlementDate = new DateTime(2024, 07, 31),
                Id = "005",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 21,
                Price = Math.Round(3024.21m / 21, ReportOptions.PriceLoadRoundingDigits),
                Total = -3024.21m,
                Currency = "USD",
                SourceIndex = 5
            },
            new () {
                SettlementDate = new DateTime(2024, 08, 01),
                Id = "006",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 22,
                Price = Math.Round(2915.84m / 22, ReportOptions.PriceLoadRoundingDigits),
                Total = -2915.84m,
                Currency = "USD",
                SourceIndex = 6
            },
            new () {
                SettlementDate = new DateTime(2024, 08, 02),
                Id = "007",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 15,
                Price = Math.Round(1538.16m / 15, ReportOptions.PriceLoadRoundingDigits),
                Total = -1538.16m,
                Currency = "USD",
                SourceIndex = 7
            },
            new () {
                SettlementDate = new DateTime(2024, 08, 09),
                Id = "008",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 23,
                Price = Math.Round(2069.26m / 23, ReportOptions.PriceLoadRoundingDigits),
                Total = -2069.26m,
                Currency = "USD",
                SourceIndex = 8
            },
            new () {
                SettlementDate = new DateTime(2024, 08, 30),
                Id = "009",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 20,
                Price = Math.Round(2267.52m / 20, ReportOptions.PriceLoadRoundingDigits),
                Total = -2267.52m,
                Currency = "USD",
                SourceIndex = 9
            },
            new () {
                SettlementDate = new DateTime(2024, 09, 06),
                Id = "010",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = 110,
                Price = Math.Round(11538.40m / 110, ReportOptions.PriceLoadRoundingDigits),
                Total = -11538.40m,
                Currency = "USD",
                SourceIndex = 10
            },
            new () {
                SettlementDate = new DateTime(2024, 09, 16),
                Id = "011",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = -45,
                Price = Math.Round(5095.42m / 45, ReportOptions.PriceLoadRoundingDigits),
                Total = 5095.42m,
                Currency = "USD",
                SourceIndex = 11
            },
            new () {
                SettlementDate = new DateTime(2024, 09, 19),
                Id = "012",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = -86,
                Price = Math.Round(10274.73m / 86, ReportOptions.PriceLoadRoundingDigits),
                Total = 10274.73m,
                Currency = "USD",
                SourceIndex = 12
            },
            new () {
                SettlementDate = new DateTime(2024, 10, 23),
                Id = "013",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 294,
                Price = Math.Round(5223.98m / 294, ReportOptions.PriceLoadRoundingDigits),
                Total = -5223.98m,
                Currency = "USD",
                SourceIndex = 13
            },
            new () {
                SettlementDate = new DateTime(2024, 11, 01),
                Id = "014",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 16,
                Price = Math.Round(2286.70m / 16, ReportOptions.PriceLoadRoundingDigits),
                Total = -2286.70m,
                Currency = "USD",
                SourceIndex = 14
            },
            new () {
                SettlementDate = new DateTime(2024, 11, 14),
                Id = "015",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 12,
                Price = Math.Round(1688.13m / 12, ReportOptions.PriceLoadRoundingDigits),
                Total = -1688.13m,
                Currency = "USD",
                SourceIndex = 15
            },
            new () {
                SettlementDate = new DateTime(2025, 01, 14),
                Id = "016",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -136,
                Price = Math.Round(4423.68m / 136, ReportOptions.PriceLoadRoundingDigits),
                Total = 4423.68m,
                Currency = "USD",
                SourceIndex = 16
            },
            new () {
                SettlementDate = new DateTime(2025, 01, 28),
                Id = "017",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 78,
                Price = Math.Round(2310.07m / 78, ReportOptions.PriceLoadRoundingDigits),
                Total = -2310.07m,
                Currency = "USD",
                SourceIndex = 17
            },
            new () {
                SettlementDate = new DateTime(2025, 03, 11),
                Id = "018",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 53,
                Price = Math.Round(1400.36m / 53, ReportOptions.PriceLoadRoundingDigits),
                Total = -1400.36m,
                Currency = "USD",
                SourceIndex = 18
            },
            new () {
                SettlementDate = new DateTime(2025, 03, 12),
                Id = "019",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 20,
                Price = Math.Round(1948.62m / 20, ReportOptions.PriceLoadRoundingDigits),
                Total = -1948.62m,
                Currency = "USD",
                SourceIndex = 19
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 07),
                Id = "020",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "DTC Inc",
                Ticker = "DTC US",
                ISIN = "USDTC",
                Quantity = -86,
                Price = Math.Round(6141.27m / 86, ReportOptions.PriceLoadRoundingDigits),
                Total = 6141.27m,
                Currency = "USD",
                SourceIndex = 20
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 07),
                Id = "021",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -289,
                Price = Math.Round(5596.17m / 289, ReportOptions.PriceLoadRoundingDigits),
                Total = 5596.17m,
                Currency = "USD",
                SourceIndex = 21
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 07),
                Id = "022",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -7,
                Price = Math.Round(609.94m / 7, ReportOptions.PriceLoadRoundingDigits),
                Total = 609.94m,
                Currency = "USD",
                SourceIndex = 22
            },
            new () {
                SettlementDate = new DateTime(2025, 04, 21),
                Id = "023",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -17,
                Price = Math.Round(1476.45m / 17, ReportOptions.PriceLoadRoundingDigits),
                Total = 1476.45m,
                Currency = "USD",
                SourceIndex = 23
            },
            new () {
                SettlementDate = new DateTime(2025, 05, 29),
                Id = "024",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -9,
                Price = Math.Round(1028.70m / 9, ReportOptions.PriceLoadRoundingDigits),
                Total = 1028.70m,
                Currency = "USD",
                SourceIndex = 24
            },
            new () {
                SettlementDate = new DateTime(2025, 05, 29),
                Id = "025",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 140,
                Price = Math.Round(5463.57m / 140, ReportOptions.PriceLoadRoundingDigits),
                Total = -5463.57m,
                Currency = "USD",
                SourceIndex = 25
            },
            new () {
                SettlementDate = new DateTime(2025, 06, 03),
                Id = "026",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 53,
                Price = Math.Round(1896.34m / 53, ReportOptions.PriceLoadRoundingDigits),
                Total = -1896.34m,
                Currency = "USD",
                SourceIndex = 26
            },
            new () {
                SettlementDate = new DateTime(2025, 06, 10),
                Id = "027",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -1,
                Price = Math.Round(51.01m / 1, ReportOptions.PriceLoadRoundingDigits),
                Total = 51.01m,
                Currency = "USD",
                SourceIndex = 27
            },
            new () {
                SettlementDate = new DateTime(2025, 06, 12),
                Id = "028",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -24,
                Price = Math.Round(1253.05m / 24, ReportOptions.PriceLoadRoundingDigits),
                Total = 1253.05m,
                Currency = "USD",
                SourceIndex = 28
            },
            new () {
                SettlementDate = new DateTime(2025, 06, 12),
                Id = "029",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -28,
                Price = Math.Round(1450.96m / 28, ReportOptions.PriceLoadRoundingDigits),
                Total = 1450.96m,
                Currency = "USD",
                SourceIndex = 29
            },
            new () {
                SettlementDate = new DateTime(2025, 06, 13),
                Id = "030",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = -26,
                Price = Math.Round(1303.25m / 26, ReportOptions.PriceLoadRoundingDigits),
                Total = 1303.25m,
                Currency = "USD",
                SourceIndex = 30
            },
            new () {
                SettlementDate = new DateTime(2025, 07, 02),
                Id = "031",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -21,
                Price = Math.Round(2875.32m / 21, ReportOptions.PriceLoadRoundingDigits),
                Total = 2875.32m,
                Currency = "USD",
                SourceIndex = 31
            },
            new () {
                SettlementDate = new DateTime(2025, 07, 22),
                Id = "032",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -21,
                Price = Math.Round(3353.28m / 21, ReportOptions.PriceLoadRoundingDigits),
                Total = 3353.28m,
                Currency = "USD",
                SourceIndex = 32
            },
            new () {
                SettlementDate = new DateTime(2025, 08, 15),
                Id = "033",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 5,
                Price = Math.Round(354.30m / 5, ReportOptions.PriceLoadRoundingDigits),
                Total = -354.30m,
                Currency = "USD",
                SourceIndex = 33
            },
            new () {
                SettlementDate = new DateTime(2025, 08, 15),
                Id = "034",
                Type = BuySell.Buy,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = 3,
                Price = Math.Round(552.02m / 3, ReportOptions.PriceLoadRoundingDigits),
                Total = -552.02m,
                Currency = "USD",
                SourceIndex = 34
            },
            new () {
                SettlementDate = new DateTime(2025, 10, 01),
                Id = "035",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "EBG NV",
                Ticker = "EBG US",
                ISIN = "USEBG",
                Quantity = 36,
                Price = Math.Round(4099.31m / 36, ReportOptions.PriceLoadRoundingDigits),
                Total = 4099.31m,
                Currency = "USD",
                SourceIndex = 35
            },
            new () {
                SettlementDate = new DateTime(2025, 11, 28),
                Id = "036",
                Type = BuySell.Sell,
                AssetType = "Equity",
                Asset = "ABC Inc",
                Ticker = "ABC US",
                ISIN = "USABC",
                Quantity = -13,
                Price = Math.Round(2755.67m / 13, ReportOptions.PriceLoadRoundingDigits),
                Total = 2755.67m,
                Currency = "USD",
                SourceIndex = 36
            }
        ];

        [SetUp]
        public void Setup()
        {
            tempFilePath_ = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(tempFilePath_);
        }

        [Test]
        public void Test01SellNoBuysFail()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[3]
            };
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[0].Id} of {transactions[0].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-transactions[0].Quantity}"));
        }

        [Test]
        public void Test02BigSellSingleSmallBuyFail()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0],
                Transactions1[6]
            };
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[1].Id} of {transactions[1].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-(transactions[1].Quantity + transactions[0].Quantity)}"));
        }

        [Test]
        public void Test03BigSellSmallBuysFail()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0],
                Transactions1[2],
                Transactions1[4],
                Transactions1[6]
            };
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[3].Id} of {transactions[3].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-(transactions[3].Quantity + transactions[2].Quantity + transactions[1].Quantity + transactions[0].Quantity)}"));
        }

        [Test]
        public void Test04BuySellWrongSequenceFail()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[3],
                Transactions1[4]
            };
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[0].Id} of {transactions[0].SettlementDate} can't be satisfied. There are no remaining buy transactions preceding the sell transaction." +
                                $"Remaining sell quantity: {-transactions[0].Quantity}. Candidate buy transaction {transactions[1].Id} of {transactions[1].SettlementDate:yyyy-MM-dd}"));
        }


        [Test]
        public void Test05SingleSellSingleBuyYearMismatchSuccess()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0],
                Transactions1[3]
            };
            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSellTransactions(tempFilePath_);
            Assert.That(sells, Is.Empty); // Sell transaction was in 2024
        }

        [Test]
        public void Test06SingleSellSingleBuySuccess()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0], // buy
                Transactions1[5]  // sell
            };
            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSellTransactions(tempFilePath_);
            var costs = Math.Round(Math.Abs(transactions[1].Quantity) * transactions[0].Total / transactions[0].Quantity, ReportOptions.CostsLoadRoundingDigits);
            var expected = new List<SellReport.SellReport.SellTransaction>
            {
                new()
                {
                    Transaction = transactions[1] with {SourceIndex = 2},
                    Costs = costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 3},
                            Costs = costs,
                            SoldQuantity = -transactions[1].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = transactions[0].Quantity + transactions[1].Quantity
                        }
                    }
                }
            };
            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test07SingleSellMultipleBuysSuccess()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0], // buy
                Transactions1[1], // buy
                Transactions1[4], // buy
                Transactions1[6]  // sell
            };
            report_.BuildReport(transactions, 2025, tempFilePath_);

            var sells = report_.LoadSellTransactions(tempFilePath_);
            var buy0Costs = transactions[0].Total; // complete sell of buy 0
            var buy1Costs = transactions[1].Total; // complete sell of buy 1
            // partial sell of buy 2
            var buy2SoldQuantity =
                (Math.Abs(transactions[3].Quantity) - transactions[0].Quantity - transactions[1].Quantity);
            var buy2Costs = buy2SoldQuantity * transactions[2].Total / transactions[2].Quantity;

            var costs = Math.Round(buy0Costs + buy1Costs + buy2Costs, ReportOptions.CostsLoadRoundingDigits);

            var expected = new List<SellReport.SellReport.SellTransaction>
            {
                new()
                {
                    Transaction = transactions[3] with {SourceIndex = 2},
                    Costs = costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 3},
                            Costs = Math.Round(buy0Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[0].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[1] with {SourceIndex = 4},
                            Costs = Math.Round(buy1Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[1].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 5},
                            Costs = Math.Round(buy2Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = buy2SoldQuantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = transactions[2].Quantity - buy2SoldQuantity
                        }
                    }
                }
            };
            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test08TwoSellsMultipleBuysSuccess()
        {
            var transactions = new List<Transaction>
            {
                Transactions1[0], // buy
                Transactions1[1], // buy
                Transactions1[2], // buy
                Transactions1[3], // sell in 2024 (accounted, but not included into a report)
                Transactions1[4], // buy
                Transactions1[6]  // sell
            };
            report_.BuildReport(transactions, 2025, tempFilePath_);

            var sells = report_.LoadSellTransactions(tempFilePath_);
            // Buy 0 partially sold in the Sell 3. The rest is sold in the Sell 5.
            var buy0SoldQuantity = transactions[0].Quantity - Math.Abs(transactions[3].Quantity);
            var buy0Costs = buy0SoldQuantity * transactions[0].Total / transactions[0].Quantity;
            var buy1Costs = transactions[1].Total; // complete sell of buy 1
            var buy2Costs = transactions[2].Total; // complete sell of buy 2
            // Buy 4 partially sold.
            var buy4SoldQuantity = Math.Abs(transactions[5].Quantity) - buy0SoldQuantity - transactions[1].Quantity -
                                   transactions[2].Quantity;
            var buy4Costs = buy4SoldQuantity * transactions[4].Total / transactions[4].Quantity;

            var costs = Math.Round(buy0Costs + buy1Costs + buy2Costs + buy4Costs, ReportOptions.CostsLoadRoundingDigits);

            var expected = new List<SellReport.SellReport.SellTransaction>
            {
                new()
                {
                    Transaction = transactions[5] with {SourceIndex = 2},
                    Costs = costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 3},
                            Costs = Math.Round(buy0Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = buy0SoldQuantity,
                            SoldEarlierQuantity =  Math.Abs(transactions[3].Quantity),
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[1] with {SourceIndex = 4},
                            Costs = Math.Round(buy1Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[1].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 5},
                            Costs = Math.Round(buy2Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[2].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[4] with {SourceIndex = 6},
                            Costs = Math.Round(buy4Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = buy4SoldQuantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = transactions[4].Quantity - buy4SoldQuantity
                        }
                    }
                }
            };
            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test09ThreeSellsMultipleBuysSuccess()
        {
            var transactions = Transactions1;
            report_.BuildReport(transactions, 2025, tempFilePath_);

            var sells = report_.LoadSellTransactions(tempFilePath_);
            // Sell 3 happened in 2024. So, it is not included in the report (but accounted in calculations).

            // Sell 5:

            // Buy 0 partially sold in the Sell 3. Then it fulfills the Sell 5.
            var sell5Costs = Math.Round(Math.Abs(transactions[5].Quantity) * transactions[0].Total / transactions[0].Quantity, ReportOptions.CostsLoadRoundingDigits);

            // Sell 6:
            // The rest of buy 0 sold in Sell 6.
            var sell6Buy0SoldQuantity = transactions[0].Quantity - Math.Abs(transactions[3].Quantity) -
                                        Math.Abs(transactions[5].Quantity);
            var sell6Costs0 = sell6Buy0SoldQuantity * transactions[0].Total / transactions[0].Quantity;
            var sell6Costs1 = transactions[1].Total; // complete sell of buy 1
            var sell6Costs2 = transactions[2].Total; // complete sell of buy 2
            var sell6Costs4 = transactions[4].Total; // complete sell of buy 4
            var sell6Costs = Math.Round(sell6Costs0 + sell6Costs1 + sell6Costs2 + sell6Costs4, ReportOptions.CostsLoadRoundingDigits);

            var expected = new List<SellReport.SellReport.SellTransaction>
            {
                new()
                {
                    Transaction = transactions[5] with {SourceIndex = 2},
                    Costs = sell5Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 3},
                            Costs = Math.Round(sell5Costs, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = Math.Abs(transactions[5].Quantity),
                            SoldEarlierQuantity =  Math.Abs(transactions[3].Quantity),
                            RemainingQuantity = sell6Buy0SoldQuantity
                        }
                    }
                },
                new()
                {
                    Transaction = transactions[6] with {SourceIndex = 4},
                    Costs = sell6Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 5},
                            Costs = Math.Round(sell6Costs0, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell6Buy0SoldQuantity,
                            SoldEarlierQuantity =  Math.Abs(transactions[3].Quantity) + Math.Abs(transactions[5].Quantity),
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[1] with {SourceIndex = 6},
                            Costs = Math.Round(sell6Costs1, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[1].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 7},
                            Costs = Math.Round(sell6Costs2, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[2].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[4] with {SourceIndex = 8},
                            Costs = Math.Round(sell6Costs4, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[4].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        }
                    }
                }
            };
            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test10ThreeInstrumentsSuccess()
        {
            var transactions = Transactions2;
            report_.BuildReport(transactions, 2025, tempFilePath_);

            var sells = report_.LoadSellTransactions(tempFilePath_);

            var expected = new List<SellReport.SellReport.SellTransaction>();

            // Sell 15 funded from buy 12
            var sell15Costs =
                Math.Round(Math.Abs(transactions[15].Quantity) * transactions[12].Total / transactions[12].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell15Costs, Is.EqualTo(-2416.5350m));
            expected.Add(
                new()
                {
                    Transaction = transactions[15] with { SourceIndex = 2 },
                    Costs = sell15Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[12] with {SourceIndex = 3},
                            Costs = sell15Costs,
                            SoldQuantity = Math.Abs(transactions[15].Quantity),
                            SoldEarlierQuantity =  0,
                            RemainingQuantity = transactions[12].Quantity - Math.Abs(transactions[15].Quantity)
                        }
                    }
                });

            // Sell 19 funded from buy 9
            var sell19Costs = Math.Round(Math.Abs(transactions[19].Quantity) * transactions[9].Total / transactions[9].Quantity,
                ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell19Costs, Is.EqualTo(-9020.9309m));
            expected.Add(
                new()
                {
                    Transaction = transactions[19] with { SourceIndex = 4 },
                    Costs = sell19Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[9] with {SourceIndex = 5},
                            Costs = sell19Costs,
                            SoldQuantity = Math.Abs(transactions[19].Quantity),
                            SoldEarlierQuantity = transactions[9].Quantity - Math.Abs(transactions[19].Quantity),
                            RemainingQuantity = 0
                        }
                    }
                });

            // Sell 20 is funded from buys 12, 16, 17
            var sell20Buy12SoldQuantity = transactions[12].Quantity - Math.Abs(transactions[15].Quantity);
            Assert.That(sell20Buy12SoldQuantity, Is.EqualTo(158));
            var sell20Costs12 = sell20Buy12SoldQuantity * transactions[12].Total / transactions[12].Quantity;
            var sell20Costs16 = transactions[16].Total;
            var sell20Costs17 = transactions[17].Total;
            var sell20Costs = Math.Round(sell20Costs12 + sell20Costs16 + sell20Costs17, ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell20Costs, Is.EqualTo(-6517.8750m));
            expected.Add(
                new()
                {
                    Transaction = transactions[20] with { SourceIndex = 6 },
                    Costs = sell20Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[12] with {SourceIndex = 7},
                            Costs = Math.Round(sell20Costs12, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell20Buy12SoldQuantity,
                            SoldEarlierQuantity = Math.Abs(transactions[15].Quantity),
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[16] with {SourceIndex = 8},
                            Costs = Math.Round(sell20Costs16, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[16].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[17] with {SourceIndex = 9},
                            Costs = Math.Round(sell20Costs17, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = transactions[17].Quantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = 0
                        }
                    }
                });

            // Sell 21 is funded from buy 0
            var sell21Costs =
                Math.Round(Math.Abs(transactions[21].Quantity) * transactions[0].Total / transactions[0].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell21Costs, Is.EqualTo(-1175.1781m));
            expected.Add(
                new()
                {
                    Transaction = transactions[21] with { SourceIndex = 10 },
                    Costs = sell21Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 11},
                            Costs = sell21Costs,
                            SoldQuantity = Math.Abs(transactions[21].Quantity),
                            SoldEarlierQuantity =  0,
                            RemainingQuantity = transactions[0].Quantity - Math.Abs(transactions[21].Quantity)
                        }
                    }
                });

            // Sell 22 is funded from buy 0
            var sell22Costs =
                Math.Round(Math.Abs(transactions[22].Quantity) * transactions[0].Total / transactions[0].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell22Costs, Is.EqualTo(-2854.0039m));
            expected.Add(
                new()
                {
                    Transaction = transactions[22] with { SourceIndex = 12 },
                    Costs = sell22Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 13},
                            Costs = sell22Costs,
                            SoldQuantity = Math.Abs(transactions[22].Quantity),
                            SoldEarlierQuantity =  Math.Abs(transactions[21].Quantity),
                            RemainingQuantity = transactions[0].Quantity - Math.Abs(transactions[21].Quantity) - Math.Abs(transactions[22].Quantity)
                        }
                    }
                });

            // Sell 23 is funded from buy 0 and buy 2
            var sell23Buy0SoldQuantity = transactions[0].Quantity - Math.Abs(transactions[21].Quantity) -
                                         Math.Abs(transactions[22].Quantity);
            Assert.That(sell23Buy0SoldQuantity, Is.EqualTo(7));
            var sell23Costs0 = sell23Buy0SoldQuantity * transactions[0].Total / transactions[0].Quantity;
            var sell23Buy2SoldQuantity = Math.Abs(transactions[23].Quantity) - sell23Buy0SoldQuantity;
            Assert.That(sell23Buy2SoldQuantity, Is.EqualTo(2));
            var sell23Costs2 = sell23Buy2SoldQuantity * transactions[2].Total / transactions[2].Quantity;
            var sell23Costs = Math.Round(sell23Costs0 + sell23Costs2, ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell23Costs, Is.EqualTo(-1529.1834m));
            expected.Add(
                new()
                {
                    Transaction = transactions[23] with { SourceIndex = 14 },
                    Costs = sell23Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[0] with {SourceIndex = 15},
                            Costs = Math.Round(sell23Costs0, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell23Buy0SoldQuantity,
                            SoldEarlierQuantity = Math.Abs(transactions[21].Quantity) + Math.Abs(transactions[22].Quantity),
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 16},
                            Costs = Math.Round(sell23Costs2, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell23Buy2SoldQuantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = transactions[2].Quantity - sell23Buy2SoldQuantity
                        },
                    }
                });

            // Sell 26 is funded from buy 24
            var sell26Costs =
                Math.Round(Math.Abs(transactions[26].Quantity) * transactions[24].Total / transactions[24].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell26Costs, Is.EqualTo(-39.0255m));
            expected.Add(
                new()
                {
                    Transaction = transactions[26] with { SourceIndex = 17 },
                    Costs = sell26Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[24] with {SourceIndex = 18},
                            Costs = sell26Costs,
                            SoldQuantity = Math.Abs(transactions[26].Quantity),
                            SoldEarlierQuantity =  0,
                            RemainingQuantity = transactions[24].Quantity - Math.Abs(transactions[26].Quantity)
                        }
                    }
                });

            // Sell 27 is funded from buy 24
            var sell27Costs =
                Math.Round(Math.Abs(transactions[27].Quantity) * transactions[24].Total / transactions[24].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell27Costs, Is.EqualTo(-936.612m));
            expected.Add(
                new()
                {
                    Transaction = transactions[27] with { SourceIndex = 19 },
                    Costs = sell27Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[24] with {SourceIndex = 20},
                            Costs = sell27Costs,
                            SoldQuantity = Math.Abs(transactions[27].Quantity),
                            SoldEarlierQuantity =  Math.Abs(transactions[26].Quantity),
                            RemainingQuantity = transactions[24].Quantity - Math.Abs(transactions[26].Quantity) - Math.Abs(transactions[27].Quantity)
                        }
                    }
                });

            // Sell 28 is funded from buy 24
            var sell28Costs =
                Math.Round(Math.Abs(transactions[28].Quantity) * transactions[24].Total / transactions[24].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell28Costs, Is.EqualTo(-1092.714m));
            expected.Add(
                new()
                {
                    Transaction = transactions[28] with { SourceIndex = 21 },
                    Costs = sell28Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[24] with {SourceIndex = 22},
                            Costs = sell28Costs,
                            SoldQuantity = Math.Abs(transactions[28].Quantity),
                            SoldEarlierQuantity =  Math.Abs(transactions[26].Quantity) + Math.Abs(transactions[27].Quantity),
                            RemainingQuantity = transactions[24].Quantity - Math.Abs(transactions[26].Quantity) - Math.Abs(transactions[27].Quantity) - Math.Abs(transactions[28].Quantity)
                        }
                    }
                });

            // Sell 29 is funded from buy 24
            var sell29Costs =
                Math.Round(Math.Abs(transactions[29].Quantity) * transactions[24].Total / transactions[24].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell29Costs, Is.EqualTo(-1014.663m));
            expected.Add(
                new()
                {
                    Transaction = transactions[29] with { SourceIndex = 23 },
                    Costs = sell29Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[24] with {SourceIndex = 24},
                            Costs = sell29Costs,
                            SoldQuantity = Math.Abs(transactions[29].Quantity),
                            SoldEarlierQuantity =  Math.Abs(transactions[26].Quantity) + Math.Abs(transactions[27].Quantity) + Math.Abs(transactions[28].Quantity),
                            RemainingQuantity = transactions[24].Quantity - Math.Abs(transactions[26].Quantity) - Math.Abs(transactions[27].Quantity) - Math.Abs(transactions[28].Quantity) - Math.Abs(transactions[29].Quantity)
                        }
                    }
                });

            // Sell 30 is funded from buy 2
            var sell30Costs =
                Math.Round(Math.Abs(transactions[30].Quantity) * transactions[2].Total / transactions[2].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell30Costs, Is.EqualTo(-3717.056m));
            expected.Add(
                new()
                {
                    Transaction = transactions[30] with { SourceIndex = 25 },
                    Costs = sell30Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 26},
                            Costs = sell30Costs,
                            SoldQuantity = Math.Abs(transactions[30].Quantity),
                            SoldEarlierQuantity = sell23Buy2SoldQuantity,
                            RemainingQuantity = transactions[2].Quantity - sell23Buy2SoldQuantity - Math.Abs(transactions[30].Quantity)
                        }
                    }
                });

            // Sell 31 is funded from buy 2 and buy 4
            var sell31Buy2SoldQuantity = transactions[2].Quantity - sell23Buy2SoldQuantity - Math.Abs(transactions[30].Quantity);
            Assert.That(sell31Buy2SoldQuantity, Is.EqualTo(7));
            var sell31Costs2 = sell31Buy2SoldQuantity * transactions[2].Total / transactions[2].Quantity;
            var sell31Buy4SoldQuantity = Math.Abs(transactions[31].Quantity) - sell31Buy2SoldQuantity;
            Assert.That(sell31Buy4SoldQuantity, Is.EqualTo(14));
            var sell31Costs4 = sell31Buy4SoldQuantity * transactions[4].Total / transactions[4].Quantity;
            var sell31Costs = Math.Round(sell31Costs2 + sell31Costs4, ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell31Costs, Is.EqualTo(-3255.1587m));
            expected.Add(
                new()
                {
                    Transaction = transactions[31] with { SourceIndex = 27 },
                    Costs = sell31Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[2] with {SourceIndex = 28},
                            Costs = Math.Round(sell31Costs2, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell31Buy2SoldQuantity,
                            SoldEarlierQuantity = sell23Buy2SoldQuantity + Math.Abs(transactions[30].Quantity),
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[4] with {SourceIndex = 29},
                            Costs = Math.Round(sell31Costs4, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell31Buy4SoldQuantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = transactions[4].Quantity - sell31Buy4SoldQuantity
                        },
                    }
                });

            // Sell 34 is funded from buy 24
            var sell34Costs =
                Math.Round(Math.Abs(transactions[34].Quantity) * transactions[24].Total / transactions[24].Quantity,
                    ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell34Costs, Is.EqualTo(-1404.918m));
            var sell34SoldEarlierQuantity = Math.Abs(transactions[26].Quantity) + Math.Abs(transactions[27].Quantity) + Math.Abs(transactions[28].Quantity) +
                                           Math.Abs(transactions[29].Quantity);
            Assert.That(sell34SoldEarlierQuantity, Is.EqualTo(79));
            var sell34RemainingQuantity = transactions[24].Quantity - sell34SoldEarlierQuantity - Math.Abs(transactions[34].Quantity);
            Assert.That(sell34RemainingQuantity, Is.EqualTo(25));
            expected.Add(
                new()
                {
                    Transaction = transactions[34] with { SourceIndex = 30 },
                    Costs = sell34Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[24] with {SourceIndex = 31},
                            Costs = sell34Costs,
                            SoldQuantity = Math.Abs(transactions[34].Quantity),
                            SoldEarlierQuantity =  sell34SoldEarlierQuantity,
                            RemainingQuantity = sell34RemainingQuantity
                        }
                    }
                });

            // Sell 35 is funded from buy 4 and buy 5
            var sell35Buy4SoldQuantity = transactions[4].Quantity - sell31Buy4SoldQuantity;
            Assert.That(sell35Buy4SoldQuantity, Is.EqualTo(7));
            var sell35Costs4 = sell35Buy4SoldQuantity * transactions[4].Total / transactions[4].Quantity;
            var sell35Buy5SoldQuantity = Math.Abs(transactions[35].Quantity) - sell35Buy4SoldQuantity;
            Assert.That(sell35Buy5SoldQuantity, Is.EqualTo(6));
            var sell35Buy5RemainingQuantity = transactions[5].Quantity - sell35Buy5SoldQuantity;
            Assert.That(sell35Buy5RemainingQuantity, Is.EqualTo(16));
            var sell35Costs5 = sell35Buy5SoldQuantity * transactions[5].Total / transactions[5].Quantity;
            var sell35Costs = Math.Round(sell35Costs4 + sell35Costs5, ReportOptions.CostsLoadRoundingDigits);
            Assert.That(sell35Costs, Is.EqualTo(-1803.2991m));
            expected.Add(
                new()
                {
                    Transaction = transactions[35] with { SourceIndex = 32 },
                    Costs = sell35Costs,
                    RelatedBuyTransactions = new List<SellReport.SellReport.RelatedBuyTransaction>
                    {
                        new()
                        {
                            Transaction = transactions[4] with {SourceIndex = 33},
                            Costs = Math.Round(sell35Costs4, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell35Buy4SoldQuantity,
                            SoldEarlierQuantity = sell31Buy4SoldQuantity,
                            RemainingQuantity = 0
                        },
                        new()
                        {
                            Transaction = transactions[5] with {SourceIndex = 34},
                            Costs = Math.Round(sell35Costs5, ReportOptions.CostsLoadRoundingDigits),
                            SoldQuantity = sell35Buy5SoldQuantity,
                            SoldEarlierQuantity = 0,
                            RemainingQuantity = sell35Buy5RemainingQuantity
                        },
                    }
                });

            ValidateSellTransactions(sells, expected);
        }

        private static void ValidateSellTransactions(List<SellReport.SellReport.SellTransaction> actual,
            List<SellReport.SellReport.SellTransaction> expected)
        {
            Assert.That(actual, Has.Exactly(expected.Count).Items);
            for (var i = 0; i < expected.Count; ++i)
            {
                TestContext.Out.WriteLine("Comparing sell transactions at index {0}", i);
                var sell = actual[i];
                var expectedSell = expected[i];
                Assert.That(sell.Transaction, Is.EqualTo(expectedSell.Transaction));
                Assert.That(sell.Costs, Is.EqualTo(expectedSell.Costs));
                Assert.That(sell.RelatedBuyTransactions, Has.Exactly(expectedSell.RelatedBuyTransactions.Count).Items);
                for (var j = 0; j < expectedSell.RelatedBuyTransactions.Count; ++j)
                {
                    TestContext.Out.WriteLine("Comparing related buy transactions at index {0}. Sell index {1}", j, i);
                    Assert.That(sell.RelatedBuyTransactions[j], Is.EqualTo(expectedSell.RelatedBuyTransactions[j]));
                }
            }
        }
    }
}
