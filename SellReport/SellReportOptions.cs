using Acm.OperationsParser;

namespace Acm.SellReport;

public class SellReportOptions
{
    public EnumNames<Columns> ColumnNames { get; } = new(new Dictionary<Columns, string>()
    {
        { Columns.SoldEarlierQuantity, "ПРОДАНО РАНЕЕ" },
        { Columns.SoldQuantity, "ПРОДАНО" },
        { Columns.RemainingQuantity, "ОСТАЛОСЬ" },
        { Columns.Costs, "РАСХОДЫ" },
        { Columns.CostsRub, "РАСХОДЫ РУБ" },
        { Columns.CurrencyRate, "КУРС ВАЛЮТЫ" }
    });

    public string ReportSheetName { get; set; } = "Продажи ЦБ";

    public int PriceRoundingDigits { get; set; } = 4;
    public int CostsRoundingDigits { get; set; } = 4;
}
