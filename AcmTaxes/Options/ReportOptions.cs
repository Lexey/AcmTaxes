using CommandLine;

namespace Acm.Taxes.Options;

[Verb("report", HelpText = "Generates transactions report from the db.")]
internal sealed class ReportOptions : CommonOptions
{
    [Value(0, Required = true, HelpText = "Reporting year")]
    public int Year { get; set; }

    [Value(1, Required = true, HelpText = "Output file path")]
    public string ReportFilePath { get; set; } = string.Empty;
}
