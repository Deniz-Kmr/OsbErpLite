using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Employees.Dtos;

namespace OsbErpLite.Application.HumanResources.Employees.Queries;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PaginatedList<EmployeeDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IEmployeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(
            request.Page, request.PageSize, request.DepartmentId, request.PositionId, cancellationToken);
        var dtos = _mapper.Map<List<EmployeeDto>>(items);
        return new PaginatedList<EmployeeDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
