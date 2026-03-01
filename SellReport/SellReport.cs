using Acm.CurrencyResolver;
using Acm.DB;
using Acm.OperationsParser;
using CodeJam;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using System.Globalization;
using Transaction = Acm.DB.Transaction;

namespace Acm.SellReport;

public class SellReport(SellReportOptions reportOptions, OperationsParserOptions operationsOptions, ICurrencyResolver currencyResolver)
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly int FirstSellColumnIndex = EnumHelper.GetEnumValues<OperationsParser.Columns>().Count + 1;

    [JsonObject(MemberSerialization.OptIn)]
    public record RelatedBuyTransaction
    {
        [JsonProperty]
        public required Transaction Transaction { get; init; }

        [JsonProperty]
        public required decimal Costs { get; set; }

        [JsonProperty]
        public required decimal CostsRub { get; set; }

        [JsonProperty]
        public required int SoldQuantity { get; set; }

        [JsonProperty]
        public required int SoldEarlierQuantity { get; set; }

        [JsonProperty]
        public required int RemainingQuantity { get; set; }
        
        [JsonProperty]
        public required decimal CurrencyRate { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public record SellTransaction
    {
        [JsonProperty]
        public required Transaction Transaction { get; init; }

        [JsonProperty]
        public required decimal Costs { get; init; }

        [JsonProperty]
        public required decimal CostsRub { get; init; }

        [JsonProperty]
        public required IReadOnlyList<RelatedBuyTransaction> RelatedBuyTransactions { get; init; }
    }

    public void BuildReport(List<Transaction> transactions, int year, string reportPath)
    {
        var sellTransactions = BuildReportData(transactions, year);
        Logger.Info("Report will include {0} sell transactions", sellTransactions.Count);
        Logger.Debug(() => $"Transactions:\r\n{JsonConvert.SerializeObject(sellTransactions, Formatting.Indented)}");
        SaveReport(sellTransactions, reportPath);
    }

    public List<SellTransaction> LoadSellTransactions(string reportPath)
    {
        using var package = new ExcelPackage(reportPath);
        var sheet = package.Workbook.Worksheets[reportOptions.ReportSheetName];
        var result = new List<SellTransaction>();
        var lastRow = sheet.Dimension.End.Row;
        var row = 2;
        while (row <= lastRow)
        {
            var transaction = ParseTransaction(sheet, row);
            if (transaction.Type != BuySell.Sell)
            {
                throw new InvalidDataException(
                    $"Unexpected transaction type {GetOperationTypeText(sheet, row)} at row {row}. Expected {operationsOptions.OperationTypeSell}");
            }

            var relatedBuyTransactions = new List<RelatedBuyTransaction>();
            var sell = new SellTransaction
            {
                Transaction = transaction,
                CostsRub = Math.Round(decimal.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CostsRub].Text), reportOptions.CostsRoundingDigits),
                Costs = Math.Round(decimal.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.Costs].Text), reportOptions.CostsRoundingDigits),
                RelatedBuyTransactions = relatedBuyTransactions
            };

            ++row;
            // related transactions
            while (row <= lastRow && GetOperationTypeText(sheet, row) == operationsOptions.OperationTypeBuy)
            {
                var buy = new RelatedBuyTransaction
                {
                    Transaction = ParseTransaction(sheet, row),
                    CostsRub = Math.Round(decimal.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CostsRub].Text), reportOptions.CostsRoundingDigits),
                    SoldQuantity = int.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.SoldQuantity].Text),
                    SoldEarlierQuantity = int.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.SoldEarlierQuantity].Text),
                    RemainingQuantity = int.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.RemainingQuantity].Text),
                    Costs = Math.Round(decimal.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.Costs].Text), reportOptions.CostsRoundingDigits),
                    CurrencyRate = decimal.Parse(sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CurrencyRate].Text)
                };
                relatedBuyTransactions.Add(buy);
                ++row;
            }
            result.Add(sell);
        }

        return result;
    }

    private List<SellTransaction> BuildReportData(List<Transaction> transactions, int year)
    {
        var transactionByISIN =
            transactions
                .Where(t => t.SettlementDate.Year <= year)
                .GroupBy(t => t.ISIN)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.SettlementDate).ThenBy(t => t.Id).ToList());

        var buyTransactions =
            transactionByISIN
                .ToDictionary(kv => kv.Key, kv => new Queue<RelatedBuyTransaction>(kv.Value.Where(t => t.Type == BuySell.Buy).Select(t => new RelatedBuyTransaction
                {
                    Transaction = t,
                    CostsRub = 0,
                    RemainingQuantity = t.Quantity,
                    SoldQuantity = 0,
                    SoldEarlierQuantity = 0,
                    Costs = 0,
                    CurrencyRate = 0
                })));

        var sellTransactions =
            transactionByISIN
                .ToDictionary(kv => kv.Key, kv => kv.Value.Where(t => t.Type == BuySell.Sell).ToList());

        var result = new List<SellTransaction>();
        foreach (var kv in sellTransactions)
        {
            var buys = buyTransactions[kv.Key];
            var usedBuys = new List<RelatedBuyTransaction>();
            foreach (var sell in kv.Value)
            {
                var includeInReport = sell.SettlementDate.Year == year;
                var totalQuantity = Math.Abs(sell.Quantity);
                var totalCost = 0m;
                var totalCostRub = 0m;
                while (totalQuantity > 0)
                {
                    if (buys.Count == 0)
                    {
                        throw new InvalidDataException(
                            $"Sell transaction {sell.Id} of {sell.SettlementDate:yyyy-MM-dd} can't be satisfied. There are no remaining buy transactions. Remaining sell quantity: {totalQuantity}");
                    }

                    var buy = buys.Peek();
                    if (buy.Transaction.SettlementDate > sell.SettlementDate || buy.Transaction.Id.CompareTo(sell.Id, StringComparison.InvariantCulture) >= 0)
                    {
                        throw new InvalidDataException(
                            $"Sell transaction {sell.Id} of {sell.SettlementDate} can't be satisfied. There are no remaining buy transactions preceding the sell transaction." +
                            $"Remaining sell quantity: {totalQuantity}. Candidate buy transaction {buy.Transaction.Id} of {buy.Transaction.SettlementDate:yyyy-MM-dd}");
                    }

                    if (buy.CurrencyRate <= 0)
                    {
                        buy.CurrencyRate = currencyResolver.Resolve(Enum.Parse<CurrencyCode>(buy.Transaction.Currency),
                            DateOnly.FromDateTime(buy.Transaction.SettlementDate));
                    }

                    var quantity = Math.Min(totalQuantity, buy.RemainingQuantity);
                    var cost = buy.Transaction.Total * quantity / buy.Transaction.Quantity;
                    var costRub = cost * buy.CurrencyRate;
                    buy.RemainingQuantity -= quantity;
                    if (includeInReport)
                    {
                        usedBuys.Add(buy with
                        {
                            Costs = cost,
                            CostsRub = costRub,
                            SoldQuantity = quantity,
                        });
                    }
                    buy.SoldEarlierQuantity += quantity;
                    totalCost += cost;
                    totalCostRub += costRub;
                    totalQuantity -= quantity;
                    if (buy.RemainingQuantity == 0)
                    {
                        buys.Dequeue();
                    }
                }

                if (!includeInReport)
                {
                    continue;
                }
                result.Add(new SellTransaction
                {
                    Costs = totalCost,
                    CostsRub = totalCostRub,
                    RelatedBuyTransactions = usedBuys,
                    Transaction = sell
                });
                usedBuys = [];
            }
        }

        return result.OrderBy(s => s.Transaction.SettlementDate).ThenBy(s => s.Transaction.Id).ToList();
    }

    private void SaveReport(List<SellTransaction> transactions, string reportPath)
    {
        Logger.Info("Saving sell transactions to {0}", reportPath);
        using var package = new ExcelPackage(reportPath);
        var sheet = package.Workbook.Worksheets.Add(reportOptions.ReportSheetName);
        sheet.OutLineSummaryBelow = false;

        // Build header
        const int headerRow = 1;
        for (var i = 1; i < FirstSellColumnIndex; ++i)
        {
            sheet.Cells[headerRow, i].Value = operationsOptions.ColumnNames[(OperationsParser.Columns)(i - 1)];
        }

        var sellColumnsCount = EnumHelper.GetEnumValues<Columns>().Count;
        for (var i = 0; i < sellColumnsCount; ++i)
        {
            sheet.Cells[headerRow, FirstSellColumnIndex + i].Value = reportOptions.ColumnNames[(Columns)i];
        }

        var row = headerRow + 1;
        foreach (var sell in transactions)
        {
            WriteTransaction(sheet, sell.Transaction, row);
            sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CostsRub].Value = Math.Round(sell.CostsRub, reportOptions.CostsRoundingDigits);
            sheet.Cells[row, FirstSellColumnIndex + (int)Columns.Costs].Value = Math.Round(sell.Costs, reportOptions.CostsRoundingDigits);
            ++row;
            var firstRelatedRow = row;
            foreach (var buy in sell.RelatedBuyTransactions)
            {
                WriteTransaction(sheet, buy.Transaction, row);
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CostsRub].Value = Math.Round(buy.CostsRub, reportOptions.CostsRoundingDigits);
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.SoldQuantity].Value = buy.SoldQuantity;
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.SoldEarlierQuantity].Value = buy.SoldEarlierQuantity;
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.RemainingQuantity].Value = buy.RemainingQuantity;
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.Costs].Value = Math.Round(buy.Costs, reportOptions.CostsRoundingDigits);
                sheet.Cells[row, FirstSellColumnIndex + (int)Columns.CurrencyRate].Value = buy.CurrencyRate;
                ++row;
            }
            sheet.Rows[firstRelatedRow, row - 1].Group();
        }

        package.SaveAs(reportPath);
    }

    private void WriteTransaction(ExcelWorksheet sheet, Transaction transaction, int row)
    {
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.SettlementDate].Value = $"{transaction.SettlementDate:yyyy-MM-dd}";
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.OperationNumber].Value = transaction.Id;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Operation].Value = operationsOptions.TransactionOperation;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.OperationType].Value = transaction.Type == BuySell.Buy ? operationsOptions.OperationTypeBuy : operationsOptions.OperationTypeSell;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.AssetType].Value = transaction.AssetType;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Asset].Value = transaction.Asset;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Ticker].Value = transaction.Ticker;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.ISIN].Value = transaction.ISIN;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Quantity].Value = transaction.Quantity;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.MeasurementUnits].Value = "units";
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Price].Value = Math.Round(transaction.Price, reportOptions.PriceRoundingDigits);
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Total].Value = transaction.Total;
        sheet.Cells[row, 1 + (int)OperationsParser.Columns.Currency].Value = transaction.Currency;
    }

    private Transaction ParseTransaction(ExcelWorksheet sheet, int row)
    {
        var type = GetOperationTypeText(sheet, row);
        return new Transaction
        {
            SettlementDate =
                DateTime.ParseExact(sheet.Cells[row, 1 + (int)OperationsParser.Columns.SettlementDate].Text,
                    "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Id = sheet.Cells[row, 1 + (int)OperationsParser.Columns.OperationNumber].Text,
            Type = type == operationsOptions.OperationTypeBuy
                ? BuySell.Buy
                : type == operationsOptions.OperationTypeSell
                    ? BuySell.Sell
                    : throw new InvalidDataException(
                        $"Unexpected transaction type {type} at row {row}"),
            AssetType = sheet.Cells[row, 1 + (int)OperationsParser.Columns.AssetType].Text,
            Asset = sheet.Cells[row, 1 + (int)OperationsParser.Columns.Asset].Text,
            Ticker = sheet.Cells[row, 1 + (int)OperationsParser.Columns.Ticker].Text,
            ISIN = sheet.Cells[row, 1 + (int)OperationsParser.Columns.ISIN].Text,
            Quantity = int.Parse(sheet.Cells[row, 1 + (int)OperationsParser.Columns.Quantity].Text),
            Price = Math.Round(decimal.Parse(sheet.Cells[row, 1 + (int)OperationsParser.Columns.Price].Text), reportOptions.PriceRoundingDigits),
            Total = decimal.Parse(sheet.Cells[row, 1 + (int)OperationsParser.Columns.Total].Text),
            Currency = sheet.Cells[row, 1 + (int)OperationsParser.Columns.Currency].Text,
            SourceIndex = row
        };
    }

    private static string GetOperationTypeText(ExcelWorksheet sheet, int row)
    {
        return sheet.Cells[row, 1 + (int)OperationsParser.Columns.OperationType].Text;
    }
}
