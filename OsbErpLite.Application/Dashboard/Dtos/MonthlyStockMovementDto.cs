namespace OsbErpLite.Application.Dashboard.Dtos;

public class MonthlyStockMovementDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalIn { get; set; }
    public int TotalOut { get; set; }
}
