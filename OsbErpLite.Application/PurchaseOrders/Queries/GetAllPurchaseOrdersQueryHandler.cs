using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.PurchaseOrders.Dtos;

namespace OsbErpLite.Application.PurchaseOrders.Queries;

public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, PaginatedList<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetAllPurchaseOrdersQueryHandler(IPurchaseOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, request.Status, cancellationToken);
        var dtos = _mapper.Map<List<PurchaseOrderDto>>(items);
        return new PaginatedList<PurchaseOrderDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
