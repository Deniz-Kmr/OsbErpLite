using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.StockMovements.Dtos;

namespace OsbErpLite.Application.StockMovements.Queries;

public class GetStockMovementsByProductIdQueryHandler : IRequestHandler<GetStockMovementsByProductIdQuery, PaginatedList<StockMovementDto>>
{
    private readonly IStockMovementRepository _repository;
    private readonly IMapper _mapper;

    public GetStockMovementsByProductIdQueryHandler(IStockMovementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<StockMovementDto>> Handle(GetStockMovementsByProductIdQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetByProductIdAsync(request.ProductId, request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<StockMovementDto>>(items);
        return new PaginatedList<StockMovementDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
