using Acm.CurrencyResolver;
using Acm.DB;
using Acm.OperationsParser;
using Acm.SellReport;
using Moq;
using Newtonsoft.Json;

namespace Acm.Tests.SellReportTests
{
    public class SellReportTests
    {
        private string tempFilePath_;

        private static readonly SellReportOptions ReportOptions = new SellReportOptions();

        private static readonly Dictionary<DateOnly, decimal> UsdCache = new()
        {
            { new DateOnly(2024, 07, 05), 88.1205m },
            { new DateOnly(2024, 07, 09), 88.1688m },
            { new DateOnly(2024, 07, 31), 86.3300m },
            { new DateOnly(2024, 08, 01), 86.1091m },
            { new DateOnly(2024, 08, 02), 85.7833m },
            { new DateOnly(2024, 08, 09), 86.5621m },
            { new DateOnly(2024, 08, 30), 91.4548m },
            { new DateOnly(2024, 09, 06), 89.7044m },
            { new DateOnly(2024, 09, 19), 91.6712m },
            { new DateOnly(2024, 10, 23), 96.5918m },
            { new DateOnly(2024, 11, 01), 97.0226m },
            { new DateOnly(2024, 11, 14), 98.3657m },
            { new DateOnly(2024, 12, 09), 99.4215m },
            { new DateOnly(2025, 01, 28), 97.1320m },
            { new DateOnly(2025, 03, 11), 88.3872m },
            { new DateOnly(2025, 03, 12), 86.5669m },
            { new DateOnly(2025, 04, 07), 84.2774m },
            { new DateOnly(2025, 05, 29), 79.6037m },
            { new DateOnly(2025, 06, 03), 79.1285m },
            { new DateOnly(2025, 08, 15), 79.7653m }
        };

        private readonly Mock<ICurrencyResolver> currencyResolver_ = new Mock<ICurrencyResolver>();

        private readonly SellReport.SellReport report_;

        public SellReportTests()
        {
            currencyResolver_.Setup(r => r.Resolve(CurrencyCode.USD, It.IsAny<DateOnly>())).Returns((CurrencyCode _,
                DateOnly date) => UsdCache[date]);
            report_ = new SellReport.SellReport(ReportOptions, new OperationsParserOptions(), currencyResolver_.Object);
        }

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
            var transactions = LoadTestData<Transaction>(nameof(Test01SellNoBuysFail), "Input.json");
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[0].Id} of {transactions[0].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-transactions[0].Quantity}"));
        }

        [Test]
        public void Test02BigSellSingleSmallBuyFail()
        {
            var transactions = LoadTestData<Transaction>(nameof(Test02BigSellSingleSmallBuyFail), "Input.json");
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[1].Id} of {transactions[1].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-(transactions[1].Quantity + transactions[0].Quantity)}"));
        }

        [Test]
        public void Test03BigSellSmallBuysFail()
        {
            var transactions = LoadTestData<Transaction>(nameof(Test03BigSellSmallBuysFail), "Input.json");
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[3].Id} of {transactions[3].SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {-(transactions[3].Quantity + transactions[2].Quantity + transactions[1].Quantity + transactions[0].Quantity)}"));
        }

        [Test]
        public void Test04BuySellWrongSequenceFail()
        {
            var transactions = LoadTestData<Transaction>(nameof(Test04BuySellWrongSequenceFail), "Input.json");
            Assert.That(() => report_.BuildReport(transactions, 2025, tempFilePath_),
                Throws.TypeOf<InvalidDataException>()
                    .And.Message.EqualTo(
                        $"Sell transaction {transactions[0].Id} of {transactions[0].SettlementDate} can't be satisfied. There are no remaining buy transactions preceding the sell transaction." +
                        $"Remaining sell quantity: {-transactions[0].Quantity}. Candidate buy transaction {transactions[1].Id} of {transactions[1].SettlementDate:yyyy-MM-dd}"));
        }


        [Test]
        public void Test05SingleSellSingleBuyYearMismatchSuccess()
        {
            const string testName = nameof(Test05SingleSellSingleBuyYearMismatchSuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test06SingleSellSingleBuySuccess()
        {
            const string testName = nameof(Test06SingleSellSingleBuySuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test07SingleSellMultipleBuysSuccess()
        {
            const string testName = nameof(Test07SingleSellMultipleBuysSuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test08TwoSellsMultipleBuysSuccess()
        {
            const string testName = nameof(Test08TwoSellsMultipleBuysSuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test09ThreeSellsMultipleBuysSuccess()
        {
            const string testName = nameof(Test09ThreeSellsMultipleBuysSuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        [Test]
        public void Test10ThreeInstrumentsSuccess()
        {
            const string testName = nameof(Test10ThreeInstrumentsSuccess);
            var transactions = LoadTestData<Transaction>(testName, "Input.json");
            var expected = LoadTestData<SellReport.SellReport.SellsOnDate>(testName, "Expected.json");

            report_.BuildReport(transactions, 2025, tempFilePath_);
            var sells = report_.LoadSells(tempFilePath_);

            ValidateSellTransactions(sells, expected);
        }

        private static List<T> LoadTestData<T>(string testName, string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "SellReportTests", "TestData", testName, fileName);
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path))
                ?? throw new InvalidDataException($"Test data file '{path}' contains null.");
        }

        private static void ValidateSellTransactions(
            List<SellReport.SellReport.SellsOnDate> actual,
            List<SellReport.SellReport.SellsOnDate> expected)
        {
            Assert.That(actual, Has.Exactly(expected.Count).Items);
            for (var i = 0; i < expected.Count; ++i)
            {
                TestContext.Out.WriteLine("Comparing SellsOneDates at index {0}", i);
                var sellsOnDate = actual[i];
                var expectedSellsOnDate = expected[i];
                Assert.That(sellsOnDate.SettlementDate, Is.EqualTo(expectedSellsOnDate.SettlementDate));
                Assert.That(sellsOnDate.CostsRub, Is.EqualTo(expectedSellsOnDate.CostsRub).Within(0.001m));
                Assert.That(sellsOnDate.Costs, Is.EqualTo(expectedSellsOnDate.Costs).Within(0.001m));
                Assert.That(sellsOnDate.SellTransactions,
                    Has.Exactly(expectedSellsOnDate.SellTransactions.Count).Items);
                for (var j = 0; j < expectedSellsOnDate.SellTransactions.Count; ++j)
                {
                    TestContext.Out.WriteLine("Comparing sell transactions at index {0}:{1}", i, j);
                    var sell = sellsOnDate.SellTransactions[j];
                    var expectedSell = expectedSellsOnDate.SellTransactions[j];
                    Assert.That(Round(sell.Transaction), Is.EqualTo(Round(expectedSell.Transaction)));
                    Assert.That(sell.CostsRub, Is.EqualTo(expectedSell.CostsRub).Within(0.0001m));
                    Assert.That(sell.Costs, Is.EqualTo(expectedSell.Costs).Within(0.0001m));
                    Assert.That(sell.RelatedBuyTransactions,
                        Has.Exactly(expectedSell.RelatedBuyTransactions.Count).Items);
                    for (var k = 0; k < expectedSell.RelatedBuyTransactions.Count; ++k)
                    {
                        TestContext.Out.WriteLine("Comparing related buy transactions at index {0}:{1}:{2}", i, j, k);
                        Assert.That(Round(sell.RelatedBuyTransactions[k]),
                            Is.EqualTo(Round(expectedSell.RelatedBuyTransactions[k])));
                    }
                }
            }
        }

        private static SellReport.SellReport.RelatedBuyTransaction Round(
            SellReport.SellReport.RelatedBuyTransaction buy)
        {
            return buy with
            {
                Costs = Math.Round(buy.Costs, 3),
                CostsRub = Math.Round(buy.CostsRub, 3),
                Transaction = Round(buy.Transaction)
            };
        }

        private static Transaction Round(Transaction t)
        {
            return t with
            {
                Price = Math.Round(t.Price, 4)
            };
        }

    }
}
