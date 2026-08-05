using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Commands;

public class ApproveSalesOrderCommandHandler : IRequestHandler<ApproveSalesOrderCommand, SalesOrderDto>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IMapper _mapper;

    public ApproveSalesOrderCommandHandler(ISalesOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SalesOrderDto> Handle(ApproveSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SalesOrder with Id '{request.Id}' was not found.");

        if (entity.Status != OrderStatus.Draft && entity.Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Sadece Draft veya Pending durumundaki siparisler onaylanabilir. Mevcut durum: {entity.Status}");

        entity.Status = OrderStatus.Approved;
        entity.ApprovedByEmployeeId = request.ApprovedByEmployeeId;

        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<SalesOrderDto>(entity);
    }
}
