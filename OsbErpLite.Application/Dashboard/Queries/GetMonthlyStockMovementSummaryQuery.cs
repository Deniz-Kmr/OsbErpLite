using MediatR;
using OsbErpLite.Application.Dashboard.Dtos;

namespace OsbErpLite.Application.Dashboard.Queries;

public record GetMonthlyStockMovementSummaryQuery : IRequest<List<MonthlyStockMovementDto>>;
