using NLog;
using OfficeOpenXml;
using System.Globalization;
using Acm.DB;

namespace Acm.OperationsParser;
public class OperationsParser(OperationsParserOptions options)
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public List<Transaction> Parse(string filePath)
    {
        using var package = new ExcelPackage(filePath);
        var sheet = package.Workbook.Worksheets[options.OperationsSheetName];
        if (sheet == null)
        {
            throw new InvalidDataException($"Worksheet {options.OperationsSheetName} not found in {filePath}");
        }

        var columnIndexes = FindColumnIndexes(sheet);
        Logger.Debug(() =>
            $"Found columns: {string.Join(",", columnIndexes.Select(kv => $"{kv.Key}: {kv.Value}"))}");

        var lastRow = sheet.Dimension.End.Row;
        var result = new List<Transaction>();
        for (var i = options.HeaderRowIndex + 1; i <= lastRow; ++i)
        {
            var operation = sheet.Cells[i, columnIndexes[Columns.Operation]].Text.Trim();
            if (operation != options.TransactionOperation)
            {
                continue;
            }
            Logger.Debug("Parsing transaction at row {0}", i);
            result.Add(ParseTransaction(sheet, i, columnIndexes));
        }
        return result;
    }

    private Dictionary<Columns, int> FindColumnIndexes(ExcelWorksheet sheet)
    {
        var nameToColumn = options.ColumnNames.ToReadOnlyDictionary().ToDictionary(kv => kv.Value, kv => kv.Key);
        var result = new Dictionary<Columns, int>();
        var lastColumn = sheet.Dimension.End.Column;
        for (var i = 1; i <= lastColumn; ++i)
        {
            var name = sheet.Cells[options.HeaderRowIndex, i].Text.Trim();
            if (nameToColumn.TryGetValue(name, out var column))
            {
                result.Add(column, i);
            }
        }

        if (result.Count != nameToColumn.Count)
        {
            throw new InvalidDataException(
                $"Failed to locate some columns in the input. Found {result.Count} expected {nameToColumn.Count}");
        }

        return result;
    }

    private Transaction ParseTransaction(ExcelWorksheet sheet, int row, Dictionary<Columns, int> columnIndexes)
    {
        var measurementUnits = sheet.Cells[row, columnIndexes[Columns.MeasurementUnits]].Text.Trim();
        if (measurementUnits != "units")
        {
            throw new InvalidDataException($"Unexpected measurement units {measurementUnits}");
        }

        var operationType = sheet.Cells[row, columnIndexes[Columns.OperationType]].Text.Trim();
        var buySell = operationType == options.OperationTypeBuy
            ? BuySell.Buy
            : operationType == options.OperationTypeSell
                ? BuySell.Sell
                : throw new InvalidDataException($"Unexpected operation type {operationType}");
        var quantity = int.Parse(sheet.Cells[row, columnIndexes[Columns.Quantity]].Text.Trim());
        var roundedPrice = decimal.Parse(sheet.Cells[row, columnIndexes[Columns.Price]].Text.Trim());
        var total = decimal.Parse(sheet.Cells[row, columnIndexes[Columns.Total]].Text.Trim());
        var realPrice = -total / quantity;
        if (Math.Abs(realPrice - roundedPrice) >= 0.01m)
        {
            throw new InvalidDataException(
                $"Calculated transaction price {realPrice} does not match the data price {roundedPrice}");
        }
        return new Transaction {
            SettlementDate = DateTime.ParseExact(sheet.Cells[row, columnIndexes[Columns.SettlementDate]].Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Id = sheet.Cells[row, columnIndexes[Columns.OperationNumber]].Text.Trim(),
            Type = buySell,
            AssetType = sheet.Cells[row, columnIndexes[Columns.AssetType]].Text.Trim(),
            Asset = sheet.Cells[row, columnIndexes[Columns.Asset]].Text.Trim(),
            Ticker = sheet.Cells[row, columnIndexes[Columns.Ticker]].Text.Trim(),
            ISIN = sheet.Cells[row, columnIndexes[Columns.ISIN]].Text.Trim(),
            Quantity = quantity,
            Price = realPrice,
            Total = total,
            Currency = sheet.Cells[row, columnIndexes[Columns.Currency]].Text.Trim(),
            SourceIndex = row
        };
    }
}
