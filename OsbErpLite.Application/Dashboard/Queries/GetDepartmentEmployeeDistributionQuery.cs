using MediatR;
using OsbErpLite.Application.Dashboard.Dtos;

namespace OsbErpLite.Application.Dashboard.Queries;

public record GetDepartmentEmployeeDistributionQuery : IRequest<List<DepartmentDistributionDto>>;
