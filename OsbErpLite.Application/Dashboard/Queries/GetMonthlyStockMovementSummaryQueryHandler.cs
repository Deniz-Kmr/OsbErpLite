using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;

namespace OsbErpLite.Application.Dashboard.Queries;

public class GetMonthlyStockMovementSummaryQueryHandler
    : IRequestHandler<GetMonthlyStockMovementSummaryQuery, List<MonthlyStockMovementDto>>
{
    private readonly IDashboardRepository _repository;

    public GetMonthlyStockMovementSummaryQueryHandler(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MonthlyStockMovementDto>> Handle(
        GetMonthlyStockMovementSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
        var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var dbData = await _repository.GetMonthlyStockMovementsAsync(startDate, cancellationToken);

        var result = new List<MonthlyStockMovementDto>();
        var current = startDate;
        var now = DateTime.UtcNow;

        while (current.Year < now.Year || (current.Year == now.Year && current.Month <= now.Month))
        {
            var existing = dbData.FirstOrDefault(d => d.Year == current.Year && d.Month == current.Month);
            result.Add(existing ?? new MonthlyStockMovementDto
            {
                Year = current.Year,
                Month = current.Month,
                TotalIn = 0,
                TotalOut = 0
            });
            current = current.AddMonths(1);
        }

        return result;
    }
}
