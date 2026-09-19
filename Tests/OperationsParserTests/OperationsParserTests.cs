using Acm.DB;
using Acm.OperationsParser;
using Newtonsoft.Json;

namespace Acm.Tests.OperationsParserTests;

public class OperationsParserTests
{
    private readonly OperationsParser.OperationsParser parser_ = new(new OperationsParserOptions());

    private static string GetDataPath(string testName, string fileName)
    {
        return Path.Combine(TestContext.CurrentContext.TestDirectory, "OperationsParserTests", "TestData", testName, fileName);
    }

    private static List<Transaction> LoadExpectedTransactions(string testName, string fileName)
    {
        var path = GetDataPath(testName, fileName);
        return JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(path))
            ?? throw new InvalidDataException($"Test data file '{path}' contains null.");
    }

    [Test]
    public void Test01Success()
    {
        const string testName = nameof(Test01Success);
        var expected = LoadExpectedTransactions(testName, "Expected.json");
        var result = parser_.Parse(GetDataPath(testName, "Input.xlsx"));
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Test02Success()
    {
        const string testName = nameof(Test02Success);
        var expected = LoadExpectedTransactions(testName, "Expected.json");
        var result = parser_.Parse(GetDataPath(testName, "Input.xlsx"));
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Test03MissingColumnFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath(nameof(Test03MissingColumnFail), "Input.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Failed to locate some columns in the input. Found 12 expected 13"));
    }

    [Test]
    public void Test04BadUnitsFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath(nameof(Test04BadUnitsFail), "Input.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Unexpected measurement units parrots"));
    }

    [Test]
    public void Test05BadOperationTypeFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath(nameof(Test05BadOperationTypeFail), "Input.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo("Unexpected operation type ЛИКВИДАЦИЯ"));
    }

    [Test]
    public void Test06PriceMismatchFail()
    {
        Assert.That(() => parser_.Parse(GetDataPath(nameof(Test06PriceMismatchFail), "Input.xlsx")),
            Throws.TypeOf<InvalidDataException>().And.Message
                .EqualTo($"Calculated transaction price {389.899m} does not match the data price {389.91m}"));
    }
}
