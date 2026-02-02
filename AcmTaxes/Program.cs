using Acm.OperationsParser;
using Acm.Taxes.Options;
using CommandLine;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using System;
using Acm.DB;
using Acm.SellReport;

namespace Acm.Taxes;

class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
        try
        {
            CommandLine.Parser.Default.ParseArguments<ImportOptions, ReportOptions>(args)
                .WithParsed<ImportOptions>(RunImport)
                .WithParsed<ReportOptions>(RunReport);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        LogManager.Shutdown();
    }

    private static void RunImport(ImportOptions options)
    {
        Logger.Info("Parsing {0}", options.FilePath);
        var parser = new OperationsParser.OperationsParser(new OperationsParserOptions());
        var transactions = parser.Parse(options.FilePath);
        Logger.Info("{0} transactions parsed", transactions.Count);
        Logger.Debug(() => $"Transactions:\r\n{JsonConvert.SerializeObject(transactions, Formatting.Indented)}");
        if (transactions.Count == 0)
        {
            Logger.Info("Nothing to do. Bye");
            return;
        }
        Logger.Info("Updating db {0}", options.DbPath);
        var updated = new Database(options.DbPath).SaveTransactions(transactions, options.Upsert);
        if (updated)
        {
            Logger.Info("Db updated");
            return;
        }
        Logger.Info("Db is already up to date");
    }

    private static void RunReport(ReportOptions options)
    {
        Logger.Info("Generating report for the year {0}", options.Year);
        var transactions = new Database(options.DbPath).LoadTransactions();
        Logger.Info("Loaded {0} transactions from the db", transactions.Count);
        new SellReport.SellReport(new SellReportOptions(), new OperationsParserOptions()).BuildReport(transactions, options.Year, options.ReportFilePath);
    }
}
