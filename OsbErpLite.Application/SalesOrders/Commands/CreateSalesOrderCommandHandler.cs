using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Commands;

public class CreateSalesOrderCommandHandler : IRequestHandler<CreateSalesOrderCommand, SalesOrderDto>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public CreateSalesOrderCommandHandler(ISalesOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SalesOrderDto> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = new SalesOrder
        {
            CustomerId = request.CustomerId,
            OrderDate = request.OrderDate,
            Status = OrderStatus.Draft,
            Lines = request.Lines.Select(l => new SalesOrderLine
            {
                ProductId = l.ProductId,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList()
        };

        entity.TotalAmount = entity.Lines.Sum(l => l.Quantity * l.UnitPrice);

        var created = await _repository.AddAsync(entity, cancellationToken);
        return _mapper.Map<SalesOrderDto>(created);
    }
}
