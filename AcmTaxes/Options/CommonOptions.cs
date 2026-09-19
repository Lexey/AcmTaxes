using CommandLine;

namespace Acm.Taxes.Options;

public class CommonOptions
{
    [Option("db", Required = false, Default = "transactions.json")]
    public string DbPath { get; set; } = "transactions.json";
}
