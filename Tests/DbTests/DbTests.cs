using Acm.DB;
using Newtonsoft.Json;

namespace Acm.Tests.DbTests
{
    public class DbTests
    {
        private string tempFilePath_;

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
            const string testName = nameof(Test01SaveLoadSuccess);
            var transactions = LoadTestData(testName, "Input.json");
            var expected = LoadTestData(testName, "Expected.json");
            var db = new Database(tempFilePath_);

            db.SaveTransactions(transactions, false);

            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        [Test]
        public void Test02SaveDuplicateTransactionIdFail()
        {
            var transactions = LoadTestData(nameof(Test02SaveDuplicateTransactionIdFail), "Input.json");
            var db = new Database(tempFilePath_);

            Assert.That(() => db.SaveTransactions(transactions, false),
                Throws.TypeOf<InvalidDataException>().And.Message.EqualTo($"There are new transactions with the same id {transactions[0].Id}"));
        }

        [Test]
        public void Test03DoubleSaveSuccess()
        {
            const string testName = nameof(Test03DoubleSaveSuccess);
            var transactions = LoadTestData(testName, "Input.json");
            var expected = LoadTestData(testName, "Expected.json");
            var db = new Database(tempFilePath_);

            db.SaveTransactions(transactions, false);
            db.SaveTransactions(transactions, false);

            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        [Test]
        public void Test04SaveModifiedAfterSaveNoUpsertFail()
        {
            const string testName = nameof(Test04SaveModifiedAfterSaveNoUpsertFail);
            var transactions = LoadTestData(testName, "Input.json");
            var db = new Database(tempFilePath_);
            db.SaveTransactions(transactions, false);
            transactions[0] = transactions[0] with { Asset = "Updated" };

            Assert.That(() => db.SaveTransactions(transactions, false),
                Throws.TypeOf<InvalidDataException>().And.Message.EqualTo($"A transaction with the same id {transactions[0].Id} but different data already exists in the Db"));
        }

        [Test]
        public void Test05SaveModifiedAfterSaveUpsertSuccess()
        {
            const string testName = nameof(Test05SaveModifiedAfterSaveUpsertSuccess);
            var transactions = LoadTestData(testName, "Input.json");
            var expected = LoadTestData(testName, "Expected.json");
            var db = new Database(tempFilePath_);

            db.SaveTransactions(transactions, false);
            transactions[0] = transactions[0] with { Asset = "Updated" };
            db.SaveTransactions(transactions, true);

            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        [Test]
        public void Test06MergeSuccess()
        {
            const string testName = nameof(Test06MergeSuccess);
            var initial = LoadTestData(testName, "Input1.json");
            var transactions = LoadTestData(testName, "Input2.json");
            var expected = LoadTestData(testName, "Expected.json");
            var db = new Database(tempFilePath_);

            db.SaveTransactions(initial, false);
            db.SaveTransactions(transactions, false);

            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        [Test]
        public void Test07MergeUpsertSuccess()
        {
            const string testName = nameof(Test07MergeUpsertSuccess);
            var disjointInput1 = LoadTestData(testName, "DisjointInput1.json");
            var disjointInput2 = LoadTestData(testName, "DisjointInput2.json");
            var commonInput = LoadTestData(testName, "CommonInput.json");
            var modifiedCommonInput = commonInput.ToArray();
            modifiedCommonInput[0] = modifiedCommonInput[0] with { Ticker = "New ticker" };
            modifiedCommonInput[1] = modifiedCommonInput[1] with { ISIN = "UPDATED" };
            var transactions1 = disjointInput1.Concat(commonInput).ToArray();
            var transactions2 = disjointInput2.Concat(modifiedCommonInput).ToArray();
            var random = new Random();
            random.Shuffle(transactions1);
            random.Shuffle(transactions2);
            var expected = LoadTestData(testName, "Expected.json");
            var db = new Database(tempFilePath_);

            db.SaveTransactions(transactions1.ToList(), false);
            db.SaveTransactions(transactions2.ToList(), true);

            Assert.That(db.LoadTransactions(), Is.EqualTo(expected));
        }

        private static List<Transaction> LoadTestData(string testName, string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "DbTests", "TestData", testName, fileName);
            return JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path))
                ?? throw new InvalidDataException($"Test data file '{path}' contains null.");
        }
    }
}
