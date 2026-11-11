using MediatR;
using OsbErpLite.Application.Dashboard.Dtos;

namespace OsbErpLite.Application.Dashboard.Queries;

public record GetDashboardSummaryQuery(bool IncludeFinancialData) : IRequest<DashboardSummaryDto>;
