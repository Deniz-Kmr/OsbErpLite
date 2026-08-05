using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, request.WarehouseId, cancellationToken);
        var dtos = _mapper.Map<List<ProductDto>>(items);
        return new PaginatedList<ProductDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
