namespace Acm.OperationsParser;

public class OperationsParserOptions
{
    /// <summary>
    /// Header row index. 1-based
    /// </summary>
    public int HeaderRowIndex { get; set; } = 3;

    public string OperationsSheetName { get; set; } = "Рассчитанные операции";

    public EnumNames<Columns> ColumnNames { get; } = new(new Dictionary<Columns, string>()
    {
        { Columns.SettlementDate, "ДАТА ОПЛАТЫ" },
        { Columns.OperationNumber, "НОМЕР ОПЕРАЦИИ" },
        { Columns.Operation, "ОПЕРАЦИЯ" },
        { Columns.OperationType, "ТИП ОПЕРАЦИИ" },
        { Columns.AssetType, "ТИП АКТИВА" },
        { Columns.Asset, "НАИМЕНОВАНИЕ АКТИВА" },
        { Columns.Ticker, "ТИКЕР" },
        { Columns.ISIN, "ИСИН" },
        { Columns.Quantity, "КОЛИЧЕСТВО" },
        { Columns.MeasurementUnits, "ЕД.ИЗМ" },
        { Columns.Price, "ЦЕНА" },
        { Columns.Total, "ИТОГО" },
        { Columns.Currency, "ВАЛЮТА" }
    });

    public string TransactionOperation { get; set; } = "Сделка";
    
    public string OperationTypeBuy { get; set; } = "ПОКУПКА";
    
    public string OperationTypeSell { get; set; } = "ПРОДАЖА";
}
