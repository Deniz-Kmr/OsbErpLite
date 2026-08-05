using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Queries;

public class GetSalesOrderByIdQueryHandler : IRequestHandler<GetSalesOrderByIdQuery, SalesOrderDto?>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetSalesOrderByIdQueryHandler(ISalesOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SalesOrderDto?> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<SalesOrderDto>(entity);
    }
}
