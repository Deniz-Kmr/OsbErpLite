using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Queries;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PaginatedList<DepartmentDto>>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IDepartmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<DepartmentDto>>(items);
        return new PaginatedList<DepartmentDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
