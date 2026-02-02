using CommandLine;

namespace Acm.Taxes.Options;

internal class CommonOptions
{
    [Option("db", Required = false, Default = "transactions.json")]
    public string DbPath { get; set; } = "transactions.json";
}
