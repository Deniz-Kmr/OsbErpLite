using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public class DeleteBusinessPartnerCommandHandler : IRequestHandler<DeleteBusinessPartnerCommand, Unit>
{
    private readonly IBusinessPartnerRepository _repository;

    public DeleteBusinessPartnerCommandHandler(IBusinessPartnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner with Id '{request.Id}' was not found.");

        // soft delete
        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
