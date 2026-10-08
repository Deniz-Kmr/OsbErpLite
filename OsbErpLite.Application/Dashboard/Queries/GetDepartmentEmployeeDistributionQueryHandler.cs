using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;

namespace OsbErpLite.Application.Dashboard.Queries;

public class GetDepartmentEmployeeDistributionQueryHandler
    : IRequestHandler<GetDepartmentEmployeeDistributionQuery, List<DepartmentDistributionDto>>
{
    private readonly IDashboardRepository _repository;

    public GetDepartmentEmployeeDistributionQueryHandler(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DepartmentDistributionDto>> Handle(
        GetDepartmentEmployeeDistributionQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetDepartmentEmployeeDistributionAsync(cancellationToken);
    }
}
