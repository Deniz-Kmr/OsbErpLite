using AutoMapper;
using MediatR;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public class CreateBusinessPartnerCommandHandler : IRequestHandler<CreateBusinessPartnerCommand, BusinessPartnerDto>
{
    private readonly IBusinessPartnerRepository _repository;
    private readonly IMapper _mapper;

    public CreateBusinessPartnerCommandHandler(IBusinessPartnerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessPartnerDto> Handle(CreateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<BusinessPartner>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);
        return _mapper.Map<BusinessPartnerDto>(created);
    }
}
