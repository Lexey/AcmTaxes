using Acm.OperationsParser;

namespace Acm.SellReport;

public class SellReportOptions
{
    public EnumNames<Columns> ColumnNames { get; } = new(new Dictionary<Columns, string>()
    {
        { Columns.SoldEarlierQuantity, "ПРОДАНО РАНЕЕ" },
        { Columns.SoldQuantity, "ПРОДАНО" },
        { Columns.RemainingQuantity, "ОСТАЛОСЬ" },
        { Columns.Costs, "РАСХОДЫ" }
    });

    public string ReportSheetName { get; set; } = "Продажи ЦБ";

    public int PriceLoadRoundingDigits { get; set; } = 4;
    public int CostsLoadRoundingDigits { get; set; } = 4;
}
