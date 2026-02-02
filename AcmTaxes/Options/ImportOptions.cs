using System;
using CommandLine;

namespace Acm.Taxes.Options;

[Verb("import", HelpText = "Imports transactions from xlsx file to the db.")]
internal sealed class ImportOptions : CommonOptions
{
    [Value(0, Required = true, HelpText = "Input xlsx file path")]
    public string FilePath { get; set; } = string.Empty;
    
    [Option("upsert", Required = false, Default = false)]
    public bool Upsert { get; set; }
}
