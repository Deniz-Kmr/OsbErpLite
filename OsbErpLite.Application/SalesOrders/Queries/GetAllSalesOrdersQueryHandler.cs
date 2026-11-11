using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Queries;

public class GetAllSalesOrdersQueryHandler : IRequestHandler<GetAllSalesOrdersQuery, PaginatedList<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetAllSalesOrdersQueryHandler(ISalesOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SalesOrderDto>> Handle(GetAllSalesOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, request.Status, cancellationToken);
        var dtos = _mapper.Map<List<SalesOrderDto>>(items);
        return new PaginatedList<SalesOrderDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
