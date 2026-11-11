using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Queries;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, PaginatedList<WarehouseDto>>
{
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public GetAllWarehousesQueryHandler(IWarehouseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<WarehouseDto>>(items);
        return new PaginatedList<WarehouseDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
