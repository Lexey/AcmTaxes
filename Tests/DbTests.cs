using Acm.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acm.Tests
{
    public class DbTests
    {
        private string tempFilePath_;

        private static readonly List<Transaction> Transactions1 =
        [
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
        ];

        private static readonly List<Transaction> Transactions2 =
        [
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
        public void Test01SaveLoadSuccess()
        {
            var db = new Database(tempFilePath_);
            db.SaveTransactions(Transactions1, false);
            Assert.That(db.LoadTransactions(), Is.EqualTo(Transactions1));
        }

        [Test]
        public void Test02SaveDuplicateTransactionIdFail()
        {
            var db = new Database(tempFilePath_);
            var transactions = new List<Transaction> { Transactions1[0], Transactions1[0] };
            Assert.That(() => db.SaveTransactions(transactions, false),
                Throws.TypeOf<InvalidDataException>().And.Message.EqualTo($"There are new transactions with the same id {transactions[0].Id}"));
        }

        [Test]
        public void Test03DoubleSaveSuccess()
        {
            var db = new Database(tempFilePath_);
            db.SaveTransactions(Transactions1, false);
            db.SaveTransactions(Transactions1, false);
            Assert.That(db.LoadTransactions(), Is.EqualTo(Transactions1));
        }

        [Test]
        public void Test04SaveModifiedAfterSaveNoUpsertFail()
        {
            var db = new Database(tempFilePath_);
            var transactions = new List<Transaction> { Transactions1[0] };
            db.SaveTransactions(transactions, false);
            transactions[0] = transactions[0] with {Asset = "Updated"};
            Assert.That(() => db.SaveTransactions(transactions, false),
                Throws.TypeOf<InvalidDataException>().And.Message.EqualTo($"A transaction with the same id {transactions[0].Id} but different data already exists in the Db"));
        }

        [Test]
        public void Test05SaveModifiedAfterSaveUpsertSuccess()
        {
            var db = new Database(tempFilePath_);
            var transactions = new List<Transaction> { Transactions1[0] };
            db.SaveTransactions(transactions, false);
            transactions[0].Asset = "Updated";
            db.SaveTransactions(transactions, true);
            Assert.That(db.LoadTransactions(), Is.EqualTo(transactions));
        }

        [Test]
        public void Test06MergeSuccess()
        {
            var db = new Database(tempFilePath_);
            db.SaveTransactions(Transactions1, false);
            var transactions2 = new List<Transaction>(Transactions2)
            {
                Transactions1[1],
                Transactions1[0]
            };
            db.SaveTransactions(Transactions2, false);
            var expected = new List<Transaction>(Transactions1);
            expected.AddRange(Transactions2);
            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        [Test]
        public void Test07MergeUpsertSuccess()
        {
            var db = new Database(tempFilePath_);
            db.SaveTransactions(Transactions1, false);
            var updated1 = Transactions1[1] with { ISIN = "UPDATED" };
            var updated2 = Transactions1[0] with { Ticker = "New ticker" };
            var transactions2 = new List<Transaction>(Transactions2)
            {
                updated1,
                updated2
            };
            db.SaveTransactions(transactions2, true);
            var expected = new List<Transaction>(Transactions1)
            {
                [0] = updated2,
                [1] = updated1
            };
            expected.AddRange(Transactions2);
            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }
    }
}
