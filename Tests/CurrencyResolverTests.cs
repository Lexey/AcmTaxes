using Acm.CurrencyResolver;

namespace Acm.Tests;

public class CurrencyResolverTests
{
    private static readonly CurrencyResolver.CurrencyResolver CurrencyResolver = new();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public decimal TestResolve(CurrencyCode code, DateOnly date)
    {
        return CurrencyResolver.Resolve(code, date);
    }

    public static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData("USD", new DateOnly(2024, 07, 05)).Returns(88.1205m);
            yield return new TestCaseData("USD", new DateOnly(2024, 07, 05)).Returns(88.1205m);
            yield return new TestCaseData("USD", new DateOnly(2024, 07, 08)).Returns(88.1348m);
            yield return new TestCaseData("USD", new DateOnly(2024, 08, 02)).Returns(85.7833m);
            yield return new TestCaseData("USD", new DateOnly(2025, 03, 12)).Returns(86.5669m);
            yield return new TestCaseData("USD", new DateOnly(2025, 03, 12)).Returns(86.5669m);
            yield return new TestCaseData("USD", new DateOnly(2025, 08, 15)).Returns(79.7653m);
            yield return new TestCaseData("AED", new DateOnly(2024, 07, 22)).Returns(23.9675m);
            yield return new TestCaseData("AED", new DateOnly(2025, 08, 02)).Returns(21.8731m);
            yield return new TestCaseData("HKD", new DateOnly(2025, 10, 17)).Returns(10.1925m);
        }
    }
}
