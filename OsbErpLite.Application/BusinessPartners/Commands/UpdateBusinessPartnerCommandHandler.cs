using AutoMapper;
using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public class UpdateBusinessPartnerCommandHandler : IRequestHandler<UpdateBusinessPartnerCommand, BusinessPartnerDto>
{
    private readonly IBusinessPartnerRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBusinessPartnerCommandHandler(IBusinessPartnerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessPartnerDto> Handle(UpdateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner with Id '{request.Id}' was not found.");

        _mapper.Map(request, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<BusinessPartnerDto>(entity);
    }
}
